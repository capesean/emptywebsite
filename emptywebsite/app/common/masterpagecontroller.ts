/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";

    angular
        .module("app")
        .controller("masterPageController", masterPageController);

    masterPageController.$inject = ["$scope", "$rootScope", "$window", "appSettings", "authService"];
    function masterPageController($scope, $rootScope, $window, appSettings, authService) {

        var vm = this;
        vm.profile = null;
        vm.logout = logout;
        vm.appSettings = appSettings;

        initPage();

        function initPage() {

            $rootScope.$watch("isLoaded", function () {
                vm.profile = $rootScope.profile;
                vm.isAdministrator = authService.isInRole(Models.Roles.Administrator);
            });

            // set page title from breadcrumb
            $scope.$watch(
                function () { return angular.element("#breadcrumb")[0].innerHTML; },
                function (newVal, oldVal) {
                    if (!newVal || newVal === oldVal) return;

                    var reg = />([^<]+)</g;
                    var result;
                    var links = [];
                    while (result = reg.exec(newVal)) {
                        if (/[a-zA-Z]/.test(result[1]))
                            links.push(result[1]);
                    };

                    if (links.length === 0) {
                        $window.document.title = appSettings.siteName;
                    } else {
                        if (links.length > 1 && links[0] === "Home") links.shift();
                        $window.document.title = links.join(": ");
                    }
                }
            );
        }

        function logout() {
            $window.location.assign("/logout");
        }
    }

})();