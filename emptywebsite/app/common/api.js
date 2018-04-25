/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    angular
        .module("app")
        .factory("settingsResource", settingsResource)
        .factory("userResource", userResource);
    //#region settings resource
    settingsResource.$inject = ["$resource", "appSettings"];
    function settingsResource($resource, appSettings) {
        return $resource(appSettings.apiServiceBaseUri + appSettings.apiPrefix + "settings");
    }
    //#endregion
    //#region user resource
    userResource.$inject = ["$resource", "appSettings"];
    function userResource($resource, appSettings) {
        return $resource(appSettings.apiServiceBaseUri + appSettings.apiPrefix + "users/:id", { id: "@id" }, {
            profile: {
                method: "GET",
                url: appSettings.apiServiceBaseUri + appSettings.apiPrefix + "users/:id/profile"
            },
            changePassword: {
                method: "POST",
                url: appSettings.apiServiceBaseUri + appSettings.apiPrefix + "users/changepassword"
            }
        });
    }
    //#endregion
}());
//# sourceMappingURL=api.js.map