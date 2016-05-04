(function () {
    'use strict';

    // проверка на наличие значения в массиве
    angular.module("gkApp")
        .directive("validateContains", function () {
            return {
                require: 'ngModel',
                link: function (scope, element, attributes, ctrl) {
                    ctrl.$parsers.push(function(value) {
                        var items = scope[attributes["validateContains"]];
                        var expression = attributes["validateCompareExpression"];
                        for (var i = 0; i < items.length; i++) {
                            if (scope.$eval(expression, items[i])) {
                                ctrl.$setValidity('contains', false);
                                return value;
                            }
                        }
                        ctrl.$setValidity('contains', true);
                        return value;
                    });
                },
                restrict: "A"
            }
        });
}());
