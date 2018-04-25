/// <reference path="../../scripts/typings/angularjs/angular.d.ts" />
(function () {
    "use strict";

    angular
        .module("app")
        .directive("filecontent", filecontent);

    function filecontent() {

        if (!FileReader.prototype.readAsBinaryString) {
            FileReader.prototype.readAsBinaryString = function (fileData) {
                var binary = "";
                var pt = this;
                var reader = new FileReader();
                reader.onload = function (e) {
                    var bytes = new Uint8Array(reader.result);
                    var length = bytes.byteLength;
                    for (var i = 0; i < length; i++) {
                        binary += String.fromCharCode(bytes[i]);
                    }
                    pt.content = binary;
                    $(pt).trigger('onload');
                }
                reader.readAsArrayBuffer(fileData);
            }
        }

        return {
            scope: {
                filecontent: "=",
                filename: "=",
                contenttype: "="
            },
            link: function (scope, element, attributes) {
                element.bind("change", function (changeEvent) {
                    var reader = new FileReader();
                    reader.onload = function (loadEvent : any) {
                        scope.$apply(function () {
                            var content = loadEvent.target.result;
                            var contentType = undefined;
                            var regex = /data:(.*);base64,(.+)/;
                            if (regex.test(content)) {
                                var matches = regex.exec(content);
                                contentType = matches[1];
                                content = matches[2];
                            }
                            scope.filecontent = content;
                            scope.filename = changeEvent.target.files[0].name;
                            scope.contenttype = contentType;
                        });
                    }
                    reader.readAsDataURL(changeEvent.target.files[0]);
                });
            }
        }
    }

})();