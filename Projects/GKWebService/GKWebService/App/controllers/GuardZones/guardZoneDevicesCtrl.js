(function () {
    angular.module('gkApp.controllers').controller('guardZoneDevicesCtrl', ['$scope', '$http', '$uibModal', 'signalrDevicesService', 'dialogService', 'constants',
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
                columnDefs: [
                   { field: 'Name', displayName: 'Устройство', width: 400, cellTemplate: '<div class="ui-grid-cell-contents" style="float:left" ><a href="" ng-click="grid.appScope.deviceClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> <img style="vertical-align: middle" width="16px" height="16px" ng-src="/Content/Image/{{row.entity.ImageSource}}" /> {{row.entity[col.field]}}</a></div>' },
                   { field: 'Address', displayName: 'Адрес', width: 100, minWidth: 150 },
                   { field: 'ActionType', displayName: 'Функция', width: 150, minWidth: 150 },
                   { field: 'Description', displayName: 'Примечание', enableColumnResizing: false }

                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = (window.innerHeight - 115) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            $scope.$on('guard', function (event, args) {
                $http.get('GuardZones/GetGuardZoneDevices/' + args).success(function (data) {
                    $scope.uiGrid.data = data;
                })
            });

            $scope.deviceClick = function (device) {
                dialogService.showWindow(constants.gkObject.device, device);
            };

            var ChangeDevices =  function (device) {
                for (var i in $scope.uiGrid.data) {
                    if ($scope.uiGrid.data[i].UID === device.UID) {
                        var actionType = $scope.uiGrid.data[i].ActionType;
                        $scope.uiGrid.data[i] = device;
                        $scope.uiGrid.data[i].ActionType = actionType;
                        break;
                    }
                }
            };

            signalrDevicesService.onDeviceChanged(function (event, args) {
                $scope.gridState = $scope.gridApi.saveState.save();
                ChangeDevices(args);
                $scope.$apply();
                $scope.gridApi.saveState.restore($scope, $scope.gridState);
            });
        }])
}());