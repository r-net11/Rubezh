(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('devicesMPTCtrl',
        ['$scope', '$http', '$uibModal', 'signalrDevicesService', 'dialogService', 'constants',
        function ($scope, $http, $uibModal, signalrDevicesService, dialogService, constants) {

            $scope.uiGrid = {
                enableRowSelection: true,
                enableRowHeaderSelection: false,
                multiSelect: false,
                modifierKeysToMultiSelect: true,
                noUnselect: true,
                enableSorting: false,
                enableColumnResizing: true,
                enableColumnMenus: false,
                columnDefs:
                  [{ field: 'MPTDeviceType', displayName: 'Тип', width: 210 },
                   { field: 'Name', displayName: 'Устройство', width: 200, cellTemplate: '<div class="ui-grid-cell-contents" style="float:left" ><a href="" ng-click="grid.appScope.deviceClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> <img style="vertical-align: middle" width="16px" height="16px" ng-src="/Content/Image/{{row.entity.ImageSource}}" /> {{row.entity[col.field]}}</a></div>' },
                   { field: 'Address', displayName: 'Адрес', width: 80 },
                   { field: 'Description', displayName: 'Описание', enableColumnResizing: false }]

            };

            $scope.gridStyle = function () {
            	var ctrlHeight = window.innerHeight - 170;
            	return "height:" + ctrlHeight + "px";
            }();

            $scope.deviceClick = function (device) {
                dialogService.showWindow(constants.gkObject.device, device);
            };

            var ChangeDevices = function (device) {
                for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                    if ($scope.uiGrid.data[i].UID === device.UID) {
                        var mptState = $scope.uiGrid.data[i].MPTDeviceType;
                        $scope.uiGrid.data[i] = device;
                        $scope.uiGrid.data[i].MPTDeviceType = mptState;
                        break;
                    }
                }
            };

            signalrDevicesService.onDeviceChanged(function (event, args) {
                ChangeDevices(args);
                $scope.$apply();
            });


            $scope.$on('mptDevicesChanged', function (event, args) {
                $http.get('MPTs/GetMPTDevices/' + args
              ).success(function (data) {
                  $scope.uiGrid.data = data;
              });
            });

        }]);
}());