(function () {
    'use strict';
    angular.module('canvasApp.directives').directive('ngJqGrid', function ($compile) {
        return {
            restrict: 'E',
            scope: {
                config: '=',
                data: '='
            },
            link: function (scope, element, attrs) {
                var table;

                scope.$watch("config", function (newValue) {

                    element.children().empty();
                    table = angular.element('<table id="' + attrs.id + '"></table>');
                    element.append($compile(table)(scope));

                    element.append(table);
                    if (newValue) {
                        angular.extend(newValue, {
                            loadComplete: function() {
                                $compile(this)(scope);
                            }
                        });
                    }

                    angular.element(table)
                        .jqGrid(newValue);
                });

                scope.$watch('data', function (newValue, oldValue) {
                    table.jqGrid("clearGridData");
                    table.jqGrid("setGridParam", { data: newValue });
                    table.trigger("reloadGrid");
                    //if (oldValue === undefined) {
                    //    table[0].addJSONData({ rows: newValue });
                    //} else {
                    //    var i;
                    //    for (i = oldValue.length - 1; i >= 0; i--) {
                    //        $(table).jqGrid('delRowData', i);
                    //    }
                    //    for (i = 0; i < newValue.length; i++) {
                    //        $(table).jqGrid('addRowData', i, newValue[i]);
                    //    }
                    //}

                });
            }
        };
    });
}());