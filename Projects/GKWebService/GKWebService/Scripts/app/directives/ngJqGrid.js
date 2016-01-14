(function () {
    'use strict';
    angular.module('canvasApp.directives').directive('ngJqGrid', function () {
        return {
            restrict: 'E',
            scope: {
                config: '=',
                data: '=',
            },
            link: function (scope, element, attrs) {
                var table;

                scope.$watch('config', function (newValue) {
                    element.children().empty();
                    table = angular.element('<table id = "grid"></table>');
                    element.append(table);
                    $(table).jqGrid(newValue);
                });

                scope.$watch('data', function (newValue, oldValue) {
                    table[0].addJSONData({rows: newValue});
                });
            }
        };
    });
}());