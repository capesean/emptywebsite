/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    angular
        .module("app")
        .controller("settings", settings);
    settings.$inject = ["$scope", "notifications", "errorService", "appSettings", "settingsResource"];
    function settings($scope, notifications, errorService, appSettings, settingsResource) {
        var vm = this;
        vm.loading = true;
        vm.settings = new settingsResource();
        vm.save = saveSettings;
        initPage();
        function initPage() {
            setModel();
            vm.loading = false;
        }
        function saveSettings() {
            if ($scope.mainForm.$invalid) {
                notifications.error("The form has not been completed correctly.", "Error");
                return;
            }
            save();
        }
        ;
        function setModel() {
            vm.settings.finalPerspectiveId = appSettings.finalPerspectiveId;
            vm.settings.performancePeerMinor = appSettings.performancePeerMinor;
            vm.settings.performancePeerMajor = appSettings.performancePeerMajor;
            vm.settings.performanceTimeMinor = appSettings.performanceTimeMinor;
            vm.settings.performanceTimeMajor = appSettings.performanceTimeMajor;
            vm.settings.capabilityTimeMinor = appSettings.capabilityTimeMinor;
            vm.settings.capabilityTimeMajor = appSettings.capabilityTimeMajor;
            vm.settings.capabilityPeerMinor = appSettings.capabilityPeerMinor;
            vm.settings.capabilityPeerMajor = appSettings.capabilityPeerMajor;
        }
        function save() {
            vm.isloading = true;
            vm.settings.$save(function (data) {
                appSettings = data;
                notifications.success("The settings has been saved.", "Saved");
                // update the global appSettings variable
                setModel();
            }, function (err) {
                errorService.handleApiError(err, "settings");
            })
                .finally(function () { return vm.loading = false; });
        }
    }
    ;
}());
//# sourceMappingURL=settings.js.map