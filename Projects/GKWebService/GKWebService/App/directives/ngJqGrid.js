(function () {
    'use strict';
    angular.module('gkApp.directives').directive('ngJqGrid', function ($compile, $rootScope) {
        return {
            restrict: 'E',
            scope: {
                config: '=',
                data: '=',
                ngdata: '='
            },
            link: function (scope, element, attrs) {
                var table;

                $rootScope.zoneNumber = 0;

                scope.$watch("config", function (newValue) {

                    element.children().empty();
                    table = angular.element('<table id="' + attrs.id + '"></table>');
                    element.append($compile(table)(scope));

                    element.append(table);
                    if (newValue) {
                        angular.extend(newValue, {
                            loadComplete: function () {
                                $compile(this)(scope);
                            },

                            onSelectRow: function (id) {
                                if (this.id === "zones") {
                                    $rootScope.zoneNumber = id;
                                    angular.element(document.getElementById('devices')).scope().main();
                                    $("#gbox_devices").remove();
                                }
                            }
                        });
                    }

                    angular.element(table)
                        .jqGrid(newValue);
                });

                scope.$watch('data', function (newValue, oldValue) {
                    table[0].addJSONData({ rows: newValue });

                    if (table[0].id == "zones") {
                        table.jqGrid('setSelection', 0);
                        $("#gbox_devices").remove();
                    }
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