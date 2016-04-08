(function () {
    'use strict';

    angular.module("gkApp")
        .directive('fileChange', function () {
            return function (scope, element, attrs, ctrl) {
                element.on('change', onChange);

                scope.$on('destroy', function () {
                    element.off('change', onChange);
                });

                function onChange() {
                    scope.$apply(function () {
                        scope.loadPhoto();
                    });
                }
            };
        });
}());


