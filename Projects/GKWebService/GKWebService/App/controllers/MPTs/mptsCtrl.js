(function () {
    'use strict';

    var app = angular.module('gkApp.controllers').controller('mptsCtrl', ['$scope', '$http', '$uibModal','signalrMPTsService',
    function ($scope, $http, $uibModal, signalrMPTsService ) {

               $http.get('MPTs/GetMPTsData').success(function (data) {
                   $scope.uiGrid.data = data;
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
                   enableRowHeaderSelection: false,
                   enableSorting: false,
                   multiSelect: false,
                   enableColumnMenus: false,
                   columnDefs:
                     [{ field: 'No', displayName: 'No', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" height="16" width="16" src="/Content/Image/Icon/GK/Blue_Direction.png" />{{row.entity[col.field]}}</div>' },
                      { field: 'Name', displayName: 'МПТ', width: 450, cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.mptClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> {{row.entity[col.field]}}</a></div>' },
                      { field: 'Delay', displayName: 'Задержка', width: 200 }],
               };

               $scope.$on('mptChanged', function (event, args) {
                   ChangeMPT(args);
                   $scope.$apply();    
               });

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
           }]);
}());