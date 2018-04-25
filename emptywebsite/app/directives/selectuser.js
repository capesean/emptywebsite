/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    angular
        .module("app")
        .controller("selectUser", selectUser);
    selectUser.$inject = ["$scope", "$uibModalInstance", "notifications", "$q", "userResource", "options"];
    function selectUser($scope, $uibModalInstance, notifications, $q, userResource, options) {
        var vm = this;
        vm.loading = true;
        vm.select = select;
        vm.cancel = cancel;
        vm.runUserSearch = runUserSearch;
        vm.options = options;
        vm.selectedUsers = options.users ? angular.copy(options.users) : [];
        vm.close = close;
        vm.clear = clear;
        vm.isSelected = isSelected;
        vm.selectAll = selectAll;
        vm.singular = options.singular || "User";
        vm.plural = options.plural || "Users";
        init();
        function init() {
            vm.search = {
                includeEntities: false,
                projectId: options.project ? options.project.projectId : undefined,
                role: options.roleId,
                subconsultants: options.subconsultant === true || false,
                current: options.current
            };
            runUserSearch(0, false);
        }
        function runUserSearch(pageIndex, dontSetLoading) {
            if (!dontSetLoading)
                vm.loading = true;
            vm.search.pageIndex = pageIndex;
            vm.search.current = vm.search.current === true ? true : undefined; // don't filter on non-current!
            // note that the ui is vm.search.subconsultants (plural), but the controller uses subconsultant
            // unchecked does not mean 'only subconsultants': it means, don't include them with the consultants
            vm.search.subconsultant = vm.search.subconsultants === true ? undefined : false;
            var promise = userResource.query(vm.search, function (data, headers) {
                vm.users = data;
                vm.userHeaders = JSON.parse(headers("X-Pagination"));
            }, function (err) {
                notifications.error("Failed to load the users.", "Error", err);
                vm.cancel();
            }).$promise;
            if (!dontSetLoading)
                promise.then(function () { return vm.loading = false; });
            return promise;
        }
        ;
        function cancel() {
            $uibModalInstance.dismiss();
        }
        function close() {
            if (!!options.multiple)
                $uibModalInstance.close(vm.selectedUsers);
            else
                $uibModalInstance.dismiss();
        }
        function clear() {
            $uibModalInstance.close(undefined);
        }
        function select(user) {
            if (!!options.multiple) {
                if (isSelected(user)) {
                    for (var i = 0; i < vm.selectedUsers.length; i++) {
                        if (vm.selectedUsers[i].id == user.id) {
                            vm.selectedUsers.splice(i, 1);
                            break;
                        }
                    }
                }
                else {
                    vm.selectedUsers.push(user);
                }
            }
            else {
                $uibModalInstance.close(user);
            }
        }
        function isSelected(user) {
            return vm.selectedUsers.filter(function (ind) { return ind.id === user.id; }).length > 0;
        }
        function selectAll() {
            vm.loading = true;
            userResource.query({
                pageSize: 0
            }, function (data) {
                $uibModalInstance.close(data);
            }, function (err) {
                notifications.error("Failed to load the users.", "Error", err);
            }).$promise.then(function () { return vm.loading = false; });
        }
    }
}());
//# sourceMappingURL=selectuser.js.map