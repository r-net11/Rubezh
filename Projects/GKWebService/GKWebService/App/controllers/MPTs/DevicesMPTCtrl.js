(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('DevicesMPTCtrl',
        ['$scope', '$http', '$uibModal', 'signalrDevicesService',
        function ($scope, $http, $uibModal, signalrDevicesService) {

            $scope.main = function (args) {

                $http.get('MPTs/GetMPTDevices/' + args
                ).success(function (data, status, headers, config) {
                    $scope.uiGrid.data = data;
                   
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

                $scope.mptClick = function (mpt) {
                    $uibModal.open({
                        animation: false,
                        templateUrl: 'MPTs/MPTDetails',
                        controller: 'mptsDetailsCtrl',
                        resolve: {
                            mpt: function () {
                                return mpt;
                            }
                        }
                    });
                };
            }

            function ChangeDevices(device) {
                for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                    if ($scope.uiGrid.data[i].UID == device.UID) {
                        $scope.uiGrid.data[i] = device;
                        break;
                    }
                }
            };

            function StartGrid()
            {
                   $scope.uiGrid = {
                    enableRowSelection: true,
                    enableRowHeaderSelection: false,
                    multiSelect: false,
                    modifierKeysToMultiSelect: true,
                    noUnselect: true,
                    enableSorting: false,
                    enableColumnResizing: true,
                    enableColumnMenus: false,
                    onRegisterApi: function (gridApi) {
                        $scope.gridApi = gridApi;
                        gridApi.selection.on.rowSelectionChanged($scope, $scope.showSelectedRow);
                        gridApi.selection.on.rowSelectionChangedBatch($scope, $scope.showSelectedRow);
                    },
                    columnDefs:
                      [{ field: 'No', displayName: 'No', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" height="16" width="16" src="/Content/Image/Icon/GK/Blue_Direction.png" />{{row.entity[col.field]}}</div>' },
                       { field: 'Name', displayName: 'МПТ', width: 450, cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.mptClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> {{row.entity[col.field]}}</a></div>' },],
                };
            }
            StartGrid();
            $scope.$on('devicesChanged', function (event, args) {
                ChangeDevices(args);
                $scope.$apply();
            });

            $scope.$on('deviceChanged', function (event, args) {
                $scope.main(args);
            });

        }]);
}());