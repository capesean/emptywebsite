/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";

    angular
        .module("app")
        .controller("user", user);

    user.$inject = ["$scope", "$state", "$stateParams", "notifications", "appSettings", "$q", "errorService", "userResource"];
    function user($scope, $state, $stateParams, notifications, appSettings, $q, errorService, userResource) {

        var vm = this;
        vm.loading = true;
        vm.appSettings = appSettings;
        vm.save = save;
        vm.delete = del;
        vm.isNew = $stateParams.id === vm.appSettings.newGuid;

        initPage();

        function initPage() {

            var promises = [];

            if (vm.isNew) {

                vm.user = new userResource();
                vm.user.id = appSettings.newGuid;
                vm.user.enabled = true;

            } else {

                promises.push(
                    userResource.get(
                        {
                            id: $stateParams.id
                        },
                        data => {
                            vm.user = data;
                        },
                        err => {

                            errorService.handleApiError(err, "user", "load");
                            $state.go("app.users");

                        }).$promise
                );

            }

            $q.all(promises).finally(() => vm.loading = false);
        }

        function save() {

            if ($scope.mainForm.$invalid) {

                notifications.error("The form has not been completed correctly.", "Error");
                return;

            }

            vm.loading = true;

            vm.user.$save(
                data => {

                    notifications.success("The user has been saved.", "Saved");
                    if (vm.isNew)
                        $state.go("app.user", {
                            id: vm.user.id
                        });

                },
                err => {

                    errorService.handleApiError(err, "user");

                }).finally(() => vm.loading = false);

        }

        function del() {

            if (!confirm("Confirm delete?")) return;

            vm.loading = true;

            userResource.delete(
                {
                    id: $stateParams.id
                },
                () => {

                    notifications.success("The user has been deleted.", "Deleted");
                    $state.go("app.users");

                },
                err => {

                    errorService.handleApiError(err, "user", "delete");

                })
                .$promise.finally(() => vm.loading = false);

        }
    };

}());
