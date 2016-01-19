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
                    table = angular.element('<table id="' + attrs.id + '"></table>');
                    element.append(table);
                    $(table).jqGrid(newValue);
                });

                scope.$watch('data', function (newValue, oldValue) {
                    var i;
                    //for (i = oldValue.length - 1; i >= 0; i--) {
                    //    $(table).jqGrid('delRowData', i);
                    //}
                    if (newValue) {
                        for (i = 0; i < newValue.length; i++) {
                            $(table).jqGrid('addRowData', i, newValue[i]);
                        }
                    }
                });
            }
        };
    });
}());