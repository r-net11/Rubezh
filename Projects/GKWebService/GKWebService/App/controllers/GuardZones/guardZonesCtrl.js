(function () {
    'use strict';
    var app = angular.module('gkApp.controllers');
    app.controller('guardZonesCtrl', ['$scope', '$http', '$timeout', '$uibModal', '$stateParams', 'signalrGuardZonesService', 'broadcastService', 'dialogService', 'constants',
    function ($scope, $http, $timeout, $uibModal, $stateParams, signalrGuardZonesService, broadcastService, dialogService, constants) {
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
                    { field: 'No', enableColumnResizing: false, displayName: '№', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/{{row.entity.ImageSource}}"/>{{row.entity[col.field]}}</div>' },
                    { field: 'Name', width: 250, displayName: 'Наименование', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.guardZoneClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png"/>{{row.entity[col.field]}}</a></div>' },
                    { field: 'SetDelay', enableColumnResizing: false, displayName: 'Задержка на постановку' },
                    { field: 'ResetDelay', displayName: 'Задержка на снятие' },
                    { field: 'AlarmDelay', enableColumnResizing: false, displayName: 'Задержка на вызов тревоги' },
                ]
            };
            $http.get('GuardZones/GetGuardZones').success(function (data) {
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

            $scope.gridStyle = function () {
            	var ctrlHeight = (window.innerHeight - 115)/2;
            	return "height:" + ctrlHeight + "px; margin-bottom:10px;";
            }();

            $scope.showSelectedRow = function (row) {
                broadcastService.send('guard', row.entity.UID);
            };

            $scope.$on('guardZoneChanged', function (event, args) {
                for (var i in $scope.uiGrid.data) {
                    if (args.UID === $scope.uiGrid.data[i].UID) {
                        $scope.uiGrid.data[i] = args;
                        $scope.$apply();
                        break;
                    }
                }
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

            $scope.guardZoneClick = function (guardZone) {
                dialogService.showWindow(constants.gkObject.guardZone, guardZone);
            };

        }]);
}());