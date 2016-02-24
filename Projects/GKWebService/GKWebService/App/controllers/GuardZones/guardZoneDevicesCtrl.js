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
                   { field: 'Name', displayName: 'Устройство', width: 450, cellTemplate: '<div class="ui-grid-cell-contents" style="float:left" ><a href="#" ng-click="grid.appScope.deviceClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> <img style="vertical-align: middle" width="16px" height="16px" ng-src="/Content/Image/{{row.entity.ImageSource}}" /> {{row.entity[col.field]}}</a></div>' },
                   { field: 'Address', displayName: 'Адрес', width: 100 },
                   { field: 'Logic', displayName: 'Логика', width: 150 },
                   { field: 'ActionType', displayName: 'Функция', width: 150 },
                   { field: 'Description', displayName: 'Описание' }

                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = (window.innerHeight - 115) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            $scope.$on('guard', function (event, args) {
                var t = 'ddd';
                $http.get('GuardZones/GetGuardZoneDevices/' + args).success(function (data) {
                    $scope.uiGrid.data = data;
                })
            });

            $scope.deviceClick = function (device) {
                dialogService.showWindow(constants.gkObject.device, device);
            };

            function ChangeDevices(device) {
                for (var i in $scope.uiGrid.data) {
                    if ($scope.uiGrid.data[i].UID === device.UID) {
                        var mptState = $scope.uiGrid.data[i].MPTDeviceType;
                        $scope.uiGrid.data[i] = device;
                        $scope.uiGrid.data[i].MPTDeviceType = mptState;
                        break;
                    }
                }
            };

            $scope.$on('devicesChanged', function (event, args) {
                ChangeDevices(args);
                $scope.$apply();
            });


        }])
}());