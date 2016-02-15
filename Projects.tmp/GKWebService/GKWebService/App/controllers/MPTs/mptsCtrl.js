(function () {
    'use strict';

    var app = angular.module('gkApp.controllers');
    app.controller('mptsCtrl', ['$scope', '$http', '$uibModal', 'signalrMPTsService','broadcastService',
    function ($scope, $http, $uibModal, signalrMPTsService, broadcastService) {

               $http.get('MPTs/GetMPTsData').success(function (data) {
                   $scope.uiGrid.data = data;
                   if ($scope.gridApi.selection.selectRow) 
                       $scope.gridApi.selection.selectRow($scope.uiGrid.data[0]);
               });

               function ChangeMPT(mpt) {
                   for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                       if ($scope.uiGrid.data[i].UID == mpt.UID) {
                           $scope.uiGrid.data[i] = mpt;
                           break;
                       }
                   }
               };

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
                     [{ field: 'No', displayName: 'No', width: 50,  cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" height="16" width="16" src="/Content/Image/Icon/GK/BMPT.png" />{{row.entity[col.field]}}</div>' },
                      { field: 'Name', displayName: 'МПТ', width: 450, cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.mptClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> {{row.entity[col.field]}}</a></div>' },
                      { field: 'Delay', displayName: 'Задержка', width: 200 }],
               };

               $scope.$on('mptChanged', function (event, args) {
                   ChangeMPT(args);
                   $scope.$apply();    
               });

               $scope.showSelectedRow = function () {
                   var uid = $scope.gridApi.selection.getSelectedRows()[0].UID;
                   $scope.selectedRow =
                   {
                       'onClausesGroup': $scope.gridApi.selection.getSelectedRows()[0].OnClausesGroup,
                       'offClausesGroup': $scope.gridApi.selection.getSelectedRows()[0].OffClausesGroup,
                       'stopClausesGroup': $scope.gridApi.selection.getSelectedRows()[0].StopClausesGroup
                   }
                   broadcastService.send('mptDevicesChanged', uid);
               };
               
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

               $scope.$on('showGKMPT', function (event, args) {
                   for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                       if ($scope.uiGrid.data[i].UID === args) {
                           $scope.gridApi.selection.selectRow($scope.uiGrid.data[i]);
                           break;
                       }
                   }
               });
    }]);
}());