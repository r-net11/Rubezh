(function () {
    'use strict';

    var app = angular.module('gkApp.controllers').controller('devicesCtrl',
        ['$scope', '$http', '$timeout', 'uiGridTreeBaseService', function ($scope, $http, $timeout, uiGridTreeBaseService) {

            var template = "<div class=\"ui-grid-cell-contents\"><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed'}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<a href=\"#\" ng-click=\"grid.appScope.fireZonesClick(row.entity)\"><img style=\"vertical-align: middle; padding-right: 3px\" ng-src=\"{{row.entity.imageState}}\"/><img style=\"vertical-align: middle; padding-right: 3px\" width=\"16px\" height=\"16px\" ng-src=\"{{row.entity.imageDevice}}\"/>{{row.entity[col.field]}}</a></div>";
            $scope.gridOptions = {
                enableSorting: false,
                enableFiltering: false,
                showTreeExpandNoChildren: false,
                enableRowHeaderSelection: false,
                enableColumnMenus: false,
                showTreeRowHeader: false,
                columnDefs: [
                    { field: 'Name', width: 300, displayName: 'Устройство', cellTemplate: template },
                    { field: 'Address', displayName: 'Адрес', width: 100 },
                    { field: 'Note', displayName: 'Примечание', width: $(window).width() - 650 }
                ]
            };

            $http.get('Devices/GetDevicesList').success(function (data, status, headers, config) {

                $scope.data = data;

                for (var i in $scope.data) {
                    var image1 = "<img src= data:image/gif;base64," + $scope.data[i].StateImageSource.Item1 + " height=16 width =16> ";
                    var image2 = "<img src= data:image/gif;base64," + $scope.data[i].ImageBloom.Item1 + " height=16 width =16>";

                    $scope.data[i].id = $scope.data[i].UID;
                    $scope.data[i].imageDevice = "data:image/gif;base64," + $scope.data[i].ImageBloom.Item1;
                    $scope.data[i].imageState = "data:image/gif;base64," + $scope.data[i].StateImageSource.Item1;
                    $scope.data[i].Name = $scope.data[i].Name;
                    $scope.data[i].$$treeLevel = $scope.data[i].Level;
                }

                $scope.gridOptions.data = $scope.data;

                $timeout(function () {
                    $scope.expandAll();
                });
            });

            $scope.expandAll = function () {
                $scope.gridApi.treeBase.expandAllRows();
            };

            $scope.toggleRow = function (row, evt) {
                uiGridTreeBaseService.toggleRowTreeState($scope.gridApi.grid, row, evt);
            };

            $scope.gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;
            };

        }]);
}());