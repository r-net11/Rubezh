(function () {
    'use strict';

    angular.module("gkApp")
        .directive('photoSelection', function () {
            return {
                require: 'ngModel',
                link: function ($scope, $element, attributes, ngModel) {
                    $scope.openClick = function() {
                        var inputFileElement = $element.find("#OpenPhoto");
                        inputFileElement.val(null);
                        inputFileElement.trigger('click');
                    };

                    $scope.loadPhoto = function() {
                        var file = $element.find("#OpenPhoto")[0].files[0];
                        if (file) {
                            if (file.size > 1200000) {
                                alert("Размер файла должен быть не больше 1 200 000 байт");
                                return;
                            }

                            var reader = new FileReader();

                            reader.onload = function (readerEvt) {
                                var binaryString = readerEvt.target.result;
                                ngModel.$setViewValue("data:image/gif;base64," + btoa(binaryString));
                            };

                            reader.readAsBinaryString(file);
                        }
                    };

                    $scope.removeClick = function () {
                        ngModel.$setViewValue("//:0");
                    };
                },
                restrict: 'E',
                transclude: false,
                templateUrl: 'Hr/PhotoSelection'
            };
        });
}());


