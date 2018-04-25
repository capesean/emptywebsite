/// <reference path="../../scripts/typings/toastr/toastr.d.ts" />
/// <reference path="../../scripts/typings/moment/moment.d.ts" />
/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    angular.module('appRoutes', []).config(appRoutes);
    appRoutes.$inject = ["$stateProvider", "$locationProvider", "$urlRouterProvider"];
    function appRoutes($stateProvider, $locationProvider, $urlRouterProvider) {
        var version = "?v=0.01";
        $locationProvider.html5Mode(true);
        $urlRouterProvider.otherwise("/");
        $stateProvider
            .state("app", {
            abstract: true,
            template: "<div ui-view></div>",
            resolve: {
                load: ["appStarter", function (appStarter) {
                        var appStarterPromise = appStarter.start();
                        return appStarterPromise.then(function () {
                            $("body").removeClass("loading bg-faded");
                        });
                    }]
            }
        }).state("app.home", {
            url: "/",
            templateUrl: "/app/home/index.html" + version,
            controller: "home",
            controllerAs: "vm",
            ncyBreadcrumb: {
                label: "Home"
            }
        }).state("app.changePassword", {
            url: "/changepassword",
            templateUrl: "/app/users/changepassword.html" + version,
            controller: "changePassword",
            controllerAs: "vm",
            ncyBreadcrumb: {
                parent: "app.home",
                label: "Change Password"
            }
        }).state("app.accessdenied", {
            url: "/accessdenied",
            templateUrl: "/app/login/denied.html" + version,
            ncyBreadcrumb: {
                skip: true
            }
        }).state("app.user", {
            url: "/users/:id",
            templateUrl: "/app/users/user.html" + version,
            controller: "user",
            controllerAs: "vm",
            ncyBreadcrumb: {
                parent: "app.users",
                label: "{{vm.user.firstName}} {{vm.user.lastName}}"
            }
        }).state("app.users", {
            url: "/users",
            templateUrl: "/app/users/users.html" + version,
            controller: "users",
            controllerAs: "vm",
            ncyBreadcrumb: {
                label: "Users"
            }
        }).state("app.settings", {
            url: "/settings",
            templateUrl: "/app/settings/settings.html" + version,
            controller: "settings",
            controllerAs: "vm",
            ncyBreadcrumb: {
                label: "Settings"
            }
            // add other routes/states here
        });
    }
})();
//# sourceMappingURL=routes.js.map