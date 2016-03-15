(function () {
    'use strict';


    var app = angular.module('gkApp.controllers');
    app.controller('pumpStationsCtrl', ['$scope', '$http', '$timeout', '$uibModal', '$stateParams', 'signalrPumpStatoinsService', 'broadcastService', 'dialogService', 'constants',
    function ($scope, $http, $timeout, $uibModal, $stateParams, signalrPumpStatoinsService, broadcastService, dialogService, constants) {

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
                $scope.gridApi.autoSize.fit($scope.uiGrid.columnDefs[0]);
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
              [{ field: 'No', displayName: '№', enableColumnResizing: false, width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" height="16" width="16" ng-src="/Content/Image/{{row.entity.ImageSource}}" />{{row.entity[col.field]}}</div>' },
               { field: 'Name', displayName: 'НС', width: 250, cellTemplate: '<div class="ui-grid-cell-contents"><a href="" ng-click="grid.appScope.pumpStationClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> {{row.entity[col.field]}}</a></div>' },
               { field: 'OnDelay', displayName: 'Задержка', width: 100 },
               { field: 'HoldDelay', displayName: 'Время тушения', enableColumnResizing: false, Width: 100 }],
        };

        $scope.gridStyle = function () {
        	var ctrlHeight = window.innerHeight - 200;
        	return "height:" + ctrlHeight + "px";
        }();

        $scope.showSelectedRow = function (row) {
           $scope.selectedRow = {
               startLogic: row.entity.StartLogic,
               stopLogic: row.entity.StopLogic,
               automaticOffLogic: row.entity.AutomaticOffLogic
            }
           broadcastService.send('pumpStationDevicesChanged', row.entity.UID);
        };

        $scope.pumpStationClick = function (pumpStation) {
            dialogService.showWindow(constants.gkObject.pumpStation, pumpStation);
        };

        $scope.$on('pumpStationsChanged', function (event, args) {
            for (var i in $scope.uiGrid.data) {
                if ($scope.uiGrid.data[i].UID == args.UID) {
                    $scope.gridState = $scope.gridApi.saveState.save();
                    $scope.uiGrid.data[i] = args;
                    $scope.$apply();
                    $scope.gridApi.saveState.restore($scope, $scope.gridState);
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
        };
    }]);
}());