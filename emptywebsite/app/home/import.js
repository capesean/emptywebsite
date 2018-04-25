/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    angular
        .module("app")
        .controller("importData", importData)
        .controller("helpModal", helpModal);
    importData.$inject = ["$scope", "$state", "$stateParams", "notifications", "appSettings", "$q", "errorService", "utilitiesResource", "$timeout", "$filter", "$uibModal", "$window"];
    function importData($scope, $state, $stateParams, notifications, appSettings, $q, errorService, utilitiesResource, $timeout, $filter, $uibModal, $window) {
        var vm = this;
        vm.loading = true;
        vm.appSettings = appSettings;
        vm.importData = importData;
        vm.cells = [];
        vm.showHelp = showHelp;
        vm.columnName = columnName;
        vm.flattenCells = function () { return [].concat.apply([], vm.cells); };
        initPage();
        function initPage() {
            $scope.$watch("vm.importFileContent", function () {
                vm.cells = [];
            });
            var promises = [];
            $q.all(promises)
                .then(function () {
                vm.loading = false;
            });
        }
        function importData() {
            vm.cells = [];
            if (!vm.importFileContent || $scope.mainForm.$invalid) {
                notifications.error("The form has not been completed correctly.", "Error");
                return;
            }
            if (vm.importFileName && vm.importFileName.length > 5 && vm.importFileName.substring(vm.importFileName.length - 4).toLowerCase() !== ".csv") {
                notifications.error("The file should have a '.csv' extension.", "Invalid File Type");
                return;
            }
            vm.loading = true;
            utilitiesResource.importData({}, {
                fileContent: vm.importFileContent
            }, function (data) {
                angular.element("input[type=file]").val(null);
                vm.importFileContent = undefined;
                $timeout(function () { vm.cells = []; });
                notifications.success("The data has been successfully imported.", "Import Data");
            }, function (err) {
                if (err.data && err.data.message) {
                    notifications.error(err.data.message, "Import Data");
                }
                else {
                    errorService.handleApiError(err, "data", "import");
                }
                if (err.data && err.data.cells && err.data.cells.length > 0) {
                    vm.cells = err.data.cells;
                    $timeout(function () { $('[data-toggle="tooltip"]').tooltip(); });
                }
            }).$promise.finally(function () {
                $scope.mainForm.$setPristine();
                vm.loading = false;
            });
        }
        function showHelp() {
            $uibModal.open({
                templateUrl: "helpmodal.html",
                controller: "helpModal",
                controllerAs: "vm",
                size: "lg"
            });
        }
        function columnName(num) {
            for (var ret = '', a = 1, b = 26; (num -= a) >= 0; a = b, b *= 26) {
                ret = String.fromCharCode(Math.floor((num % b) / a) + 65) + ret;
            }
            return ret;
        }
    }
    ;
    helpModal.$inject = ["$scope", "$uibModalInstance"];
    function helpModal($scope, $uibModalInstance) {
        var vm = this;
        vm.loading = true;
        vm.cancel = function () { return $uibModalInstance.dismiss("cancel"); };
        init();
        function init() {
            vm.loading = false;
        }
    }
}());
//# sourceMappingURL=import.js.map