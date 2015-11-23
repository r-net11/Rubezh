(function () {
    'use strict';

    var app = angular.module('canvasApp.controllers').controller('firezonesCtrl', [
        '$scope', function ($scope) {

            $scope.config = {
                datatype: "local",
                height: 150,
                colNames: ['Устройство', 'Адрес', 'Логика', 'Примечание'],
                colModel:
                    [{ name: 'device', index: 'device', width: 250, sortable: false },
                    { name: 'address', index: 'address', width: 100, sortable: false },
                    { name: 'logic', index: 'logic', width: 50, sortable: false },
                    { name: 'note', index: 'note', width: 780, sortable: false }]
            };

            $scope.data =
                [{ device: "device1", address: "addr1", logic: "", note: "note1" },
                { device: "device2", address: "addr2", logic: "", note: "note2" },
                { device: "device3", address: "addr3", logic: "", note: "note3" },
                { device: "device4", address: "addr4", logic: "", note: "note4" },
                { device: "device5", address: "addr5", logic: "", note: "note5" }];
        }
    ]);



    app.directive('ngJqGrid', function () {
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
                    table = angular.element('<table></table>');
                    element.append(table);
                    $(table).jqGrid(newValue);
                });

                scope.$watch('data', function (newValue, oldValue) {
                    var i;
                    for (i = oldValue.length - 1; i >= 0; i--) {
                        $(table).jqGrid('delRowData', i);
                    }
                    for (i = 0; i < newValue.length; i++) {
                        $(table).jqGrid('addRowData', i, newValue[i]);
                    }
                });
            }
        };
    });


}());