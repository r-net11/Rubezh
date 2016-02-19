(function () {
    'use strict';


    var app = angular.module('gkApp.controllers');
    app.controller('pumpStationsCtrl', ['$scope', '$http', '$timeout', '$uibModal', '$stateParams', 'signalrPumpStatoinsService', 'broadcastService',
    function ($scope, $http, $timeout, $uibModal, $stateParams, signalrPumpStatoinsService, broadcastService) {

        $http.get('PumpStations/GetPumpStations').success(function (data) {
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
              [{ field: 'No', displayName: '№', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" height="16" width="16" src="/Content/Image/Icon/GK/BMPT.png" />{{row.entity[col.field]}}</div>' },
               { field: 'Name', displayName: 'НС', width: 450, cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.pumpStationClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> {{row.entity[col.field]}}</a></div>' },
               { field: 'Delay', displayName: 'Задержка', width: 200 },
               { field: 'Hold', displayName: 'Время тушения', widtd: 200 }],
        };

        $scope.showSelectedRow = function (row) {
           $scope.selectedRow = {
               startLogic: row.entity.StartLogic,
               stopLogic: row.entity.StopLogic,
               automaticOffLogic: row.entity.AutomaticOffLogic
            }
           broadcastService.send('pumpStationDevicesChanged', row.entity.UID);
        };

        $scope.pumpStationClick = function (pumpStation) {
            $uibModal.open({
                animation: false,
                templateUrl: 'PumpStations/PumpStationDetails',
                controller: 'pumpStationDetailsCtrl',
                backdrop: false,
                resolve: {
                    pumpStation: function () {
                        return pumpStation;
                    }
                }
            });
        };

        $scope.$on('pumpStationsChanged', function (event, args) {
            for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                if ($scope.uiGrid.data[i].UID == args.UID) {
                    $scope.uiGrid.data[i] = args;
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
        };

        $scope.$on('showPumpStationDetails', function (event, args) {
            for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                if ($scope.uiGrid.data[i].UID === args) {
                    $scope.pumpStationClick($scope.uiGrid.data[i]);
                    break;
                }
            }
        });

    }]);

}());