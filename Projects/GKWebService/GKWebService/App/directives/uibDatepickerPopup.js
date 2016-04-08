(function () {
    'use strict';

    angular.module("gkApp")
        .directive('uibDatepickerPopup', function () {
        return {
            require: 'ngModel',
            priority: 9999,
            link: function (scope, element, attrs, ngModel) {
                ngModel.$formatters.push(function (val) {
                    return new Date(val);
                });
            }
        };
    });
}());