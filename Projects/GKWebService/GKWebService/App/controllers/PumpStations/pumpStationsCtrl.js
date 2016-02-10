(function () {
    'use strict';


    var app = angular.module('gkApp.controllers');
    app.controller('pumpStationsCtrl', ['$scope', '$http', '$uibModal', 'signalrPumpStatoinsService', 'broadcastService',
    function ($scope, $http, $uibModal, signalrPumpStatoinsService, broadcastService) {

        $http.get('PumpStations/GetPumpStations').success(function (data) {
            $scope.uiGrid.data = data;
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

        $scope.showSelectedRow = function () {
            var uid = $scope.gridApi.selection.getSelectedRows()[0].UID;
           $scope.selectedRow = {
                startLogic : $scope.gridApi.selection.getSelectedRows()[0].StartLogic,
                stopLogic: $scope.gridApi.selection.getSelectedRows()[0].StopLogic,
                automaticOffLogic: $scope.gridApi.selection.getSelectedRows()[0].AutomaticOffLogic
            }
            broadcastService.send('pumpStationDevicesChanged', uid);
        };

        $scope.pumpStationClick = function (pumpStation) {
            $uibModal.open({
                animation: false,
                templateUrl: 'PumpStations/PumpStationDetails',
                controller: 'pumpStationDetailsCtrl',
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