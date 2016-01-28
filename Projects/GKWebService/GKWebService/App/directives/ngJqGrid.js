(function () {
    'use strict';
    angular.module('canvasApp.directives').directive('ngJqGrid', function ($compile) {
        return {
            restrict: 'E',
            scope: {
                config: '=',
                data: '=',
                ngdata: '='
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
                    table[0].addJSONData({ rows: newValue });
                });

                scope.$watch('ngdata', function (newValue, oldValue) {
                    table.jqGrid("clearGridData");
                    table.jqGrid("setGridParam", { data: newValue });
                    table.trigger("reloadGrid");
                });
            }
        };
    });
}());