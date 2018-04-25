/// <reference path="../../scripts/typings/toastr/toastr.d.ts" />
/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    var appSettings = {
        apiServiceBaseUri: window.location.toString().substring(0, window.location.pathname === "/" ? window.location.toString().length - 1 : window.location.toString().indexOf(window.location.pathname)) + "/",
        apiPrefix: "api/",
        // for searching enums by id
        find: function (array, id) { return array.filter(function (o) { return o.id === id; })[0]; },
        formatDate: function (input) { return input ? moment(input).format("YYYY-MM-DD") : ""; },
        newGuid: "00000000-0000-0000-0000-000000000000",
        createGuid: function () {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                    .toString(16)
                    .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                s4() + '-' + s4() + s4() + s4();
        },
        newInt: "0",
        trimTo: function (text, length) {
            if (!text || text.length <= length)
                return text;
            if (text.substring(0, length).indexOf(" ") <= 0)
                return text.substring(0, length) + "...";
            return text.substring(0, text.substring(0, length).lastIndexOf(" ")) + "...";
        },
        getEnumLabel: function (enums, id) {
            var foundEnum = undefined;
            angular.forEach(enums, function (e) {
                if (e.id === id)
                    foundEnum = e;
            });
            return foundEnum ? foundEnum.label : undefined;
        }
    };
    angular
        .module("app", [
        "ui.router",
        "ngResource",
        "ui.bootstrap",
        "ngMessages",
        "ncy-angular-breadcrumb",
        "nya.bootstrap.select",
        "ngSanitize",
        "LocalStorageModule",
        "ui.sortable",
        "appRoutes",
        "entityRoutes",
        "duScroll",
        "ngFileUpload",
        "angular-loading-bar" // loading bar
    ])
        .config(config)
        .factory("notifications", notificationFactory)
        .factory("appStarter", appStarter)
        .constant("appSettings", appSettings)
        .run(run);
    appStarter.$inject = ["settingsResource", "notifications", "$timeout", "authService", "$window", "userResource", "$q", "$rootScope"];
    function appStarter(settingsResource, notifications, $timeout, authService, $window, userResource, $q, $rootScope) {
        return {
            start: function () {
                if (/internet explorer/.test($window.navigator.userAgent)) {
                    if (confirm("It appears you are using an out of date version of Internet Explorer, which might result in errors using this website. Would you like to upgrade your browser?")) {
                        $window.location.href = "https://support.microsoft.com/en-za/help/18520/download-internet-explorer-11-offline-installer";
                        return;
                    }
                }
                var promises = [];
                promises.push(userResource.profile({}, function (data) {
                    var profile = {};
                    for (var k in data) {
                        if (data.hasOwnProperty(k) && k.substring(0, 1) !== "$")
                            profile[k] = data[k];
                    }
                    $rootScope.profile = profile;
                }, function (err) {
                    notifications.error("Failed to load the user profile", "Initialization error");
                }).$promise);
                promises.push(settingsResource.get({}, function (data) {
                    for (var k in data) {
                        if (data.hasOwnProperty(k) && k.substring(0, 1) !== "$")
                            appSettings[k] = data[k];
                    }
                }, function (err) {
                    notifications.error("Failed to load the application settings", "Initialization error");
                }).$promise);
                $q.all(promises).then(function () {
                    $rootScope.isLoaded = true;
                }, function (err) {
                    window.setTimeout(function () {
                        $window.location.assign("/logout");
                    }, 1000);
                });
                return $q.all(promises);
            }
        };
    }
    ;
    config.$inject = ["uibDatepickerConfig", "uibDatepickerPopupConfig", "$urlRouterProvider", "$stateProvider", "$locationProvider", "$httpProvider", "$breadcrumbProvider", "$uibTooltipProvider"];
    function config(uibDatepickerConfig, uibDatepickerPopupConfig, $urlRouterProvider, $stateProvider, $locationProvider, $httpProvider, $breadcrumbProvider, $uibTooltipProvider) {
        // convert dates
        $httpProvider.defaults.transformResponse.push(function (responseData) {
            var dateRegEx = /^([1-2]\d{3})-(0[1-9]|1[0-2])-([0-2][0-9]|3[0-1])T([0-1]\d|2[0-3]):([0-5]\d):([0-5]\d)$/;
            function convertDateStringsToDates(input) {
                if (typeof input !== "object")
                    return input;
                for (var key in input) {
                    if (!input.hasOwnProperty(key))
                        continue;
                    var value = input[key];
                    var match;
                    // check for string properties which look like dates
                    if (typeof value === "string" && (match = value.match(dateRegEx))) {
                        var milliseconds = Date.parse(match[0]);
                        if (!isNaN(milliseconds)) {
                            // remove any timezone offset
                            var _date = new Date(match[1], parseInt(match[2]) - 1, match[3], match[4], match[5], match[6]);
                            _date = new Date(_date.getTime() - (_date.getTimezoneOffset() * 60 * 1000));
                            input[key] = _date;
                        }
                    }
                    else if (typeof value === "object") {
                        // recurse into object
                        convertDateStringsToDates(value);
                    }
                }
            }
            convertDateStringsToDates(responseData);
            return responseData;
        });
        // setup breadcrumb
        $breadcrumbProvider.setOptions({
            prefixStateName: "app.home",
            template: "bootstrap4"
        });
        // setup date picker
        uibDatepickerConfig.startingDay = 1;
        uibDatepickerConfig.showWeeks = false;
        uibDatepickerConfig.formatYear = "yyyy";
        // setup tooltip
        $uibTooltipProvider.options({ appendToBody: true });
    }
    run.$inject = ["$timeout", "$window", "$rootScope", "$state", "notifications", "authService", "authorization", "$document"];
    function run($timeout, $window, $rootScope, $state, notifications, authService, authorization, $document) {
        $rootScope.$on("$stateChangeStart", function (event, toState, toStateParams) {
            // from: http://stackoverflow.com/questions/22537311/angular-ui-router-login-authentication
            $rootScope.toState = toState;
            $rootScope.toStateParams = toStateParams;
        });
        $rootScope.$on("$stateChangeSuccess", function (event, current, previous) {
            $timeout(function () {
                $document.scrollTo(0, 0, 400);
                $('[data-toggle="tooltip"]').tooltip();
            }, 250);
        });
        $rootScope.$on('$stateChangeError', function (event, toState, toParams, fromState, fromParams, error) {
            if (error.status === -1)
                notifications.error("Unable to communicate with the website.", "Connection Issue");
            else
                notifications.error("An unexpected error was encountered.", "State Change Error");
        });
        // set default options for toastr
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "progressBar": false,
            "positionClass": "toast-bottom-right",
            "onclick": null,
            "showDuration": 300,
            "hideDuration": 1000,
            "timeOut": 5000,
            "extendedTimeOut": 1000,
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
    }
    function notificationFactory() {
        return {
            success: function (text, title) {
                toastr.success(text, title);
            },
            info: function (text, title) {
                toastr.info(text, title);
            },
            warning: function (text, title) {
                toastr.warning(text, title);
            },
            error: function (text, title, err) {
                console.log(title, text, err);
                toastr.error(text, title);
            }
        };
    }
}());
//# sourceMappingURL=app.js.map