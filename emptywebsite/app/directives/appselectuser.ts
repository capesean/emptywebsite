/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";

    angular
        .module("app")
        .directive("appSelectUser", appSelectUser);

    function appSelectUser() {

        return {
            templateUrl: "/app/directives/selectuser.html",
            restrict: "E",
            controller: selectUserController,
            replace: true,
            scope: {
                multiple: "<",
                user: "=",
                ngModel: "=",
                ngDisabled: "<",
                singular: "<",
                plural: "<",
                roleId: "<"
            }
        }
    }

    selectUserController.$inject = ["$scope", "$uibModal", "appSettings"];
    function selectUserController($scope, $uibModal, appSettings) {

        $scope.selectUser = selectUser;
        $scope.placeholder = "Select " + ($scope.singular ? $scope.singular.toLowerCase() : "user");

        function selectUser() {
            var modalInstance = $uibModal.open({
                templateUrl: "/app/directives/selectuser.html",
                controller: "selectUser",
                controllerAs: "vm",
                size: "lg",
                resolve: {
                    options: () => {
                        return {
                            multiple: $scope.multiple,
                            user: $scope.user,
                            singular: $scope.singular,
                            plural: $scope.plural,
                            roleId: $scope.roleId
                        }
                    },
                }
            });

            modalInstance.result.then(
                function (user) {
                    $scope.user = user;
                    $scope.ngModel = user ? user.id : undefined;
                },
                function (reason) {
                    // cancelled/closed
                });
        }
    }

})();