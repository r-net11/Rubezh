(function () {
    'use strict';
    var app = angular.module('gkApp.controllers');
    app.controller('guardZonesCtrl', ['$scope', '$http', '$timeout', '$uibModal', '$stateParams', 'signalrGuardZonesService', 'broadcastService',
    function ($scope, $http, $timeout, $uibModal, $stateParams, signalrGuardZonesService, broadcastService) {
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
                    { field: 'Name', width: 200, displayName: 'Наименование', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.guardZoneClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png"/>{{row.entity[col.field]}}</a></div>' },
                    { field: 'SetDelay', enableColumnResizing: false, displayName: 'Задержка на постановку' },
                    { field: 'ResetDelay', enableColumnResizing: false, displayName: 'Задержка на снятие' },
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
                        break;
                    }
                }
            }

            $scope.guardZoneClick = function (guardZone) {
                var modalInstance = $uibModal.open({
                    animation: false,
                    size: 'rbzh',
                    templateUrl: 'GuardZones/GuardZoneDetails',
                    controller: 'guardZoneDetailsCtrl',
                    resolve: {
                        guardZone: function () {
                            return guardZone;
                        }
                    }
                });
            };

        }]);
}());