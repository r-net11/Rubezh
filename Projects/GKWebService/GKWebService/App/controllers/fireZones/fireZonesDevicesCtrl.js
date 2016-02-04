(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('fireZonesDevicesCtrl',
        ['$scope', '$http', '$timeout', '$uibModal', 'uiGridTreeViewConstants', 'uiGridTreeBaseService', 'signalrFireZonesService',
        function ($scope, $http, $timeout, $uibModal, uiGridTreeViewConstants, uiGridTreeBaseService) {

            var template = "<div class=\"ui-grid-cell-contents\"><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) ) && row.treeNode.state === 'expanded'}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<a href=\"#\" ng-click=\"grid.appScope.fireZonesClick(row.entity)\"><img style=\"vertical-align: middle; padding-right: 3px\" ng-src=\"{{row.entity.imageState}}\"/><img style=\"vertical-align: middle\" width=\"16px\" height=\"16px\" ng-src=\"{{row.entity.imageDevice}}\"/> {{row.entity[col.field]}}</a></div>";
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
                    { field: 'Description', displayName: 'Примечание', width: $(window).width() - 675 }
                ]
            };

            $scope.expandAll = function () {
                $scope.gridApi.treeBase.expandAllRows();
            };

            $scope.toggleRow = function (row, evt) {
                uiGridTreeBaseService.toggleRowTreeState($scope.gridApi.grid, row, evt);
            };

            $scope.gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;
            };

            $scope.$on('fireZonesDevicesChanged', function (event, args) {
                var data = $scope.gridOptions.data;
                for (var i = 0, len = data.length; i < len; i++) {
                    if (args.Uid === data[i].Uid) {
                        data[i] = args;
                        $scope.$apply();
                        break;
                    }
                }
            });

            $scope.fireZonesDevicesClick = function (fireZoneDevice) {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'FireZones/FireZonesDevicesDetails',
                    controller: 'fireZonesDevicesDetailsCtrl',
                    resolve: {
                        fireZoneDevice: function () {
                            return fireZoneDevice;
                        }
                    }
                });
            };

            $scope.$on('selectedZoneChanged', function (event, args) {
                $http({
                    url: 'FireZones/GetDevicesListByZoneNumber/',
                    method: "GET",
                    params: { uid: args }
                }).success(function (data, status, headers, config) {
                    $scope.gridOptions.data = [];
                    for (var i in data) {
                        var item = data[i].DeviceList;
                        for (var j in item) {
                            var element = item[j];
                            $scope.gridOptions.data.push({
                                Name: element.Name,
                                Address: element.Address,
                                $$treeLevel: data.length - element.Level - 1,
                                imageDevice: element.ImageDeviceIcon,
                                imageState: element.StateIcon,
                                Description: element.Description
                            });
                        }
                    }
                    //Раскрываем дерево после загрузки
                    $timeout(function () {
                        $scope.expandAll();
                    });
                });
            });

        }]);
}());