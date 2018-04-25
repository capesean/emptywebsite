/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";
    angular
        .module("app")
        .directive("textFileReader", textFileReader);
    function textFileReader() {
        return {
            scope: {
                textFileReader: "="
            },
            link: function (scope, element) {
                $(element).on('change', function (changeEvent) {
                    scope.$apply(function () {
                        scope.textFileReader = undefined;
                    });
                    var files = changeEvent.target.files;
                    if (files.length) {
                        // only allow csv files. make this a parameter?
                        if (!files[0].name.toLowerCase().endsWith(".csv"))
                            return;
                        var r = new FileReader();
                        r.onload = function (e) {
                            var contents = e.target.result;
                            scope.$apply(function () {
                                scope.textFileReader = contents;
                            });
                        };
                        r.readAsText(files[0]);
                    }
                });
            }
        };
    }
    ;
})();
//# sourceMappingURL=textfilereader.js.map