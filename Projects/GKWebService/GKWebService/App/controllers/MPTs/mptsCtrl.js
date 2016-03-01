(function () {
    'use strict';

    var app = angular.module('gkApp.controllers');
    app.controller('mptsCtrl', ['$scope', '$http', '$timeout', '$uibModal', '$stateParams', 'signalrMPTsService', 'broadcastService', 'dialogService', 'constants',
    function ($scope, $http, $timeout, $uibModal, $stateParams, signalrMPTsService, broadcastService, dialogService, constants) {

               $http.get('MPTs/GetMPTsData').success(function (data) {
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
               	var ctrlHeight = window.innerHeight - 170;
               	return "height:" + ctrlHeight + "px";
               }();

               var ChangeMPT = function (mpt) {
                   for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                       if ($scope.uiGrid.data[i].UID === mpt.UID) {
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
                     [{ field: 'No', displayName: 'No', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" height="16" width="16" src="/Content/Image/{{row.entity.ImageSource}}" />{{row.entity[col.field]}}</div>' },
                      { field: 'Name', displayName: 'МПТ', width: 300, cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.mptClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> {{row.entity[col.field]}}</a></div>' },
                      { field: 'OnDelay', displayName: 'Задержка', enableColumnResizing: false, minWidth:100 }],
               };

               $scope.$on('mptChanged', function (event, args) {
                   ChangeMPT(args);
                   $scope.$apply();    
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

               $scope.showSelectedRow = function (row) {
                   $scope.selectedRow =
                   {
                       onClausesGroup: row.entity.OnClausesGroup,
                       offClausesGroup: row.entity.OffClausesGroup,
                       stopClausesGroup: row.entity.StopClausesGroup
                   }
                   broadcastService.send('mptDevicesChanged', row.entity.UID);
               };
               
               $scope.mptClick = function (mpt) {
                   dialogService.showWindow(constants.gkObject.mpt, mpt);
               };
    }]);
}());