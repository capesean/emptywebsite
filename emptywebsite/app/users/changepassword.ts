/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";

    angular
        .module("app")
        .controller("changePassword", changePassword);

    changePassword.$inject = ["$scope", "$state", "$stateParams", "userResource", "notifications", "appSettings", "$q", "errorService"];
    function changePassword($scope, $state, $stateParams, userResource, notifications, appSettings, $q, errorService) {

        var vm = this;
        vm.loading = true;
        vm.appSettings = appSettings;
        vm.password = {};
        vm.save = changePassword;

        initPage();

        function initPage() {
            vm.loading = false;
        }

        function changePassword() {

            if ($scope.mainForm.$invalid) {

                notifications.error("The form has not been completed correctly.", "Error");
                return;

            }

            vm.loading = true;

            userResource.changePassword(
                {
                    currentPassword: vm.password.currentPassword,
                    newPassword: vm.password.newPassword,
                    repeatPassword: vm.password.repeatPassword
                },
                data => {

                    notifications.success("The password has been changed.", "Change Password");
                    vm.password = {};
                    $scope.mainForm.$setPristine();

                },
                err => {

                    errorService.handleApiError(err, "password", "change");

                }).$promise.finally(() => vm.loading = false);

        };
    };

}()); 