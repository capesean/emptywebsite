/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    angular
        .module("app")
        .controller("home", home);
    home.$inject = ["$scope", "$state", "$q", "$timeout", "notifications", "appSettings", "$rootScope", "$uibModal", "authService", "$filter"];
    function home($scope, $state, $q, $timeout, notifications, appSettings, $rootScope, $uibModal, authService, $filter) {
        var vm = this;
        vm.profile = $rootScope.profile;
        vm.loading = true;
        vm.appSettings = appSettings;
        vm.moment = moment;
        initPage();
        function initPage() {
            var promises = [];
            $q.all(promises)
                .then(function () {
                vm.loading = false;
            });
        }
    }
    ;
}());
//# sourceMappingURL=index.js.map