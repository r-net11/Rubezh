(function () {
    'use strict';

    // проверка на наличие значения в массиве
    angular.module("gkApp")
        .directive("uniqueElement", function () {
            return {
                require: 'ngModel',
                link: function (scope, element, attributes, ctrl) {
                    ctrl.$parsers.push(function(value) {
                        var items = scope[attributes["uniqueElement"]];
                        var id = scope[attributes["ngModel"].split('.')[0]].UID;
                        var expression = attributes["ngModel"].split('.')[1];
                        for (var i = 0; i < items.length; i++) {
                            var item = items[i];
                            if (scope.$eval(expression, item) === value && item.UID !== id && !item.IsDeleted) {
                                ctrl.$setValidity('unique', false);
                                return value;
                            }
                        }
                        ctrl.$setValidity('unique', true);
                        return value;
                    });
                },
                restrict: "A"
            }
        });
}());
