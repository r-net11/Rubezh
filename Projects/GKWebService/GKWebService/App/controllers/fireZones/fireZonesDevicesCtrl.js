(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('fireZonesDevicesCtrl',
        ['$scope', '$http', '$timeout', '$uibModal', 'uiGridConstants', 'uiGridTreeViewConstants', 'uiGridTreeBaseService', 'signalrFireZonesService',
        function ($scope, $http, $timeout, $uibModal, uiGridConstants, uiGridTreeViewConstants, uiGridTreeBaseService) {

            var template = "<div class=\"ui-grid-cell-contents\"><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) ) && row.treeNode.state === 'expanded'}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<a href=\"#\" ng-click=\"grid.appScope.fireZonesDevicesClick(row.entity)\"><img style=\"vertical-align: middle; padding-right: 3px\" ng-src=\"/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png\"/><img style=\"vertical-align: middle\" width=\"16px\" height=\"16px\" ng-src=\"/Content/Image/{{row.entity.ImageSource}}\"/> {{row.entity[col.field]}}</a></div>";
            $scope.gridOptions = {
                enableSorting: false,
                enableFiltering: false,
                showTreeExpandNoChildren: false,
                enableRowHeaderSelection: false,
                enableColumnMenus: false,
                showTreeRowHeader: false,
                enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
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


            function ChangeDevices(device) {
                for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                    if ($scope.gridOptions.data[i].UID === device.UID) {
                        $scope.gridOptions.data[i].ImageSource = device.ImageSource;
                        $scope.gridOptions.data[i].StateIcon = device.StateIcon;
                        break;
                    }
                }
            };

            $scope.$on('devicesChanged', function (event, args) {
                ChangeDevices(args);
                $scope.$apply();
            });

            $scope.fireZonesDevicesClick = function (device) {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Devices/DeviceDetails',
                    controller: 'fireZonesDevicesDetailsCtrl',
                    size: 'rbzh',
                    resolve: {
                        device: function () {
                            return device;
                        }
                    }
                });
            };

            $scope.$on('selectedZoneChanged', function (event, args) {
                $http.get('FireZones/GetDevicesByZoneUID/' + args
                ).success(function (data, status, headers, config) {
                    $scope.gridOptions.data = [];
                    for (var i in data) {
                        var item = data[i].DeviceList;
                        for (var j in item) {
                            var element = item[j];
                            $scope.gridOptions.data.push({
                                Name: element.Name,
                                Address: element.Address,
                                $$treeLevel: data.length - element.Level - 1,
                                ImageSource: element.ImageSource,
                                StateIcon: element.StateIcon,
                                Description: element.Description,
                                UID: element.UID,
                                State: element.State,
                                GKDescriptorNo: element.GKDescriptorNo,
                                Properties: element.Properties,
                                MeasureParameters: element.MeasureParameters,
                                IsBiStateControl: element.IsBiStateControl,
                                IsTriStateControl: element.IsTriStateControl,
                                HasReset: element.HasReset,
                                CanSetIgnoreState: element.CanSetIgnoreState,
                                CanSetAutomaticState: element.CanSetAutomaticState,
                                CanReset: element.CanReset,
                                ControlRegimeIcon: element.ControlRegimeIcon,
                                ControlRegimeName: element.ControlRegimeName
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