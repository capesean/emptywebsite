/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    angular
        .module("app")
        .controller("users", users);
    users.$inject = ["$scope", "$state", "appSettings", "notifications", "userResource", "$q"];
    function users($scope, $state, appSettings, notifications, userResource, $q) {
        var vm = this;
        vm.loading = true;
        vm.appSettings = appSettings;
        vm.search = {};
        vm.runSearch = runSearch;
        vm.goToUser = function (userId) { return $state.go("app.user", { id: userId }); };
        initPage();
        function initPage() {
            var promises = [];
            $q.all(promises).finally(function () { return runSearch(0); });
        }
        function runSearch(pageIndex) {
            vm.loading = true;
            userResource.query({
                pageIndex: pageIndex,
                searchText: vm.search.text,
                roleId: vm.search.roleId,
                includeEntities: true
            }, function (data, headers) {
                vm.headers = JSON.parse(headers("X-Pagination"));
                vm.users = data;
            }, function (err) {
                notifications.error("Failed to load the users.", "Error", err);
                $state.go("app.home");
            }).$promise.finally(function () { return vm.loading = false; });
        }
        ;
    }
    ;
}());
//# sourceMappingURL=users.js.map