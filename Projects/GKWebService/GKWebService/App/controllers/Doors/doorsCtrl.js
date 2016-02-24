(function () {
    var app = angular.module('gkApp.controllers');
    app.controller('doorsCtrl',
        ['$scope', '$http', '$timeout', '$uibModal', '$stateParams', 'signalrDoorsService', 'broadcastService', 'dialogService', 'constants',
        function ($scope, $http, $timeout, $uibModal, $stateParams, signalrDoorsService, broadcastService, dialogService, constants) {
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

                columnDefs: [
                    { field: 'No', enableColumnResizing: false, displayName: '№', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/{{row.entity.ImageSource}}" />{{row.entity[col.field]}}</div>' },
                    { field: 'Name', width: 200, displayName: 'Наименование', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.doorClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png"/>{{row.entity[col.field]}}</a></div>' },
                    { field: 'DoorTypeString', width: Math.round(($(window).width() - 525) / 2), displayName: 'Тип точки доступа' },
                    { field: 'Desription', width: Math.round(($(window).width() - 525) / 2), displayName: 'Примечание' }
                ]
            };

            $scope.gridStyle = function () {
            	var ctrlHeight = window.innerHeight - 265;
            	return "height:" + ctrlHeight + "px";
            }();

            $scope.showSelectedRow = function (row) {
            }

            $scope.$on('doorChanged', function (event, args) {
                for (var i in $scope.uiGrid.data) {
                    if (args.UID === $scope.uiGrid.data[i].UID) {
                        $scope.uiGrid.data[i] = args;
                        $scope.$apply();
                        break;
                    }
                }
            });

            $scope.showSelectedRow = function (row) {

                $scope.selectedRow = {
                    OneWay: row.entity.DoorType === 0,
                    TwoWay: row.entity.DoorType === 1,
                    Turnstile: row.entity.DoorType === 2,
                    Barrier: row.entity.DoorType === 3,
                    AirlockBooth: row.entity.DoorType === 4,
                    exitDevice: row.entity.ExitDevice,
                    exitDeviceShow: !!row.entity.ExitDevice,
                    enterDevice: row.entity.EnterDevice,
                    enterDeviceShow: !!row.entity.EnterDevice,
                    enterButton: row.entity.EnterButton,
                    enterButtonShow: !!row.entity.EnterButton,
                    exitButton: row.entity.ExitButton,
                    exitButtonShow: !!row.entity.ExitButton,
                    lockDevice: row.entity.LockDevice,
                    lockDeviceShow: !!row.entity.LockDevice,
                    lockDeviceExit: row.entity.LockDeviceExit,
                    lockDeviceExitShow: !!row.entity.LockDeviceExit,
                    lockControlDevice: row.entity.LockControlDevice,
                    lockControlDeviceShow: !!row.entity.LockControlDevice,
                    lockControlDeviceExit: row.entity.LockControlDeviceExit,
                    lockControlDeviceExitShow: !!row.entity.LockControlDeviceExit,
                    enterZone: row.entity.EnterZone,
                    enterZoneShow: !!row.entity.EnterZone,
                    exitZone: row.entity.ExitZone,
                    exitZoneShow: !!row.entity.ExitZone,
                    openRegimeLogic: row.entity.OpenRegimeLogic,
                    normRegimeLogic: row.entity.NormRegimeLogic,
                    closeRegimeLogic: row.entity.CloseRegimeLogic
                }
            };

            $scope.ShowDevice = function (value) {
                //if ((enterZoneShow || exitZoneShow) && (value === enterZone.Item2 || value == exitZone.Item2))
                $state.go('device', { uid: value });
            }

            //$scope.getDescription = function () {
            //    if ($scope.selectedRow !== undefined) {
            //        if ($scope.selectedRow.OneWay || $scope.selectedRow.TwoWay)
            //            return 'Замок: ';
            //        if ($scope.selectedRow.AirlockBooth || $scope.selectedRow.Turnstile)
            //            return 'Реле на вход: ';
            //    }
            //};

            //$scope.$on('deviceChanged', function (event, args) {
            //    if(  $scope.selectedRow.exitDeviceShow && $scope.selectedRow.exitDevice.UID === args.UID)
            //        deviceChange($scope.selectedRow.exitDeviceShow, args)

            });

            //function deviceChange(obj1, obj2) {
                
            //}

            $scope.doorClick = function (door) {
                dialogService.showWindow(constants.gkObject.door, door);
            };

            $http.get('Doors/GetDoors').success(function (data) {
                $scope.uiGrid.data = data;

                $timeout(function () {
                    if ($stateParams.uid) {
                        $scope.selectRowById($stateParams.uid);
                    } else {
                        if ($scope.gridApi.selection.selectRow) {
                            $scope.gridApi.selection.selectRow($scope.uiGrid.data[0]);
                        }
                    }
                });
            });

            $scope.selectRowById = function (uid) {
                for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                    if ($scope.uiGrid.data[i].UID === uid) {
                        $scope.gridApi.selection.selectRow($scope.uiGrid.data[i]);
                        $scope.gridApi.core.scrollTo($scope.uiGrid.data[i], $scope.uiGrid.columnDefs[0]);
                        break;
                    }
                }
            }
        }]);
}());