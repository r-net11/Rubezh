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
                    },
                    columnDefs:
                      [{ field: 'MPTDeviceType', displayName: 'Тип', width: 100 },
                       { field: 'Name', displayName: 'Устройство', width: 450, cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.mptClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> <img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> {{row.entity[col.field]}}</a></div>' },
                       { field: 'DottedPresentationAddress', displayName: 'Адрес', width: 100 },
                       { field: 'Description', displayName: 'Описание', width: 100 }]

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