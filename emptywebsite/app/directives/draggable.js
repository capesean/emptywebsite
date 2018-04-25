/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    angular.module("app")
        .directive('draggable', function () {
        return {
            restrict: 'A',
            link: function (scope, elm, attrs) {
                var options = scope.$eval(attrs.andyDraggable); //allow options to be passed in
                elm.draggable(options);
            }
        };
    });
})();
//# sourceMappingURL=draggable.js.map