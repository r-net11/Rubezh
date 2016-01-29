(function () {
    'use strict';
    var app = angular.module('gkApp.controllers').controller('mptsCtrl', ['$scope', '$http', '$uibModal', 'signalrMPTsService',
           function ($scope, $http, $uibModal, signalrMPTsService) {

               function ChangeMPT(mpt) {
                   for (var i = 0; i < $scope.uiGrid.data.length; i++) {
                       if ($scope.uiGrid.data[i].UID == mpt.UID) {
                           $scope.uiGrid.data[i] = mpt;
                       }
                   }
               }

               $scope.uiGrid = {
                   enableColumnResizing: true,
                   columnDefs:
                     [{ field: 'No', displayName: 'No', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Delay.png"/>{{row.entity.No}}</div>' },
                      { field: 'Name', displayName: 'МПТ', width: 450, cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.mptClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> {{row.entity.Name}}</a></div>' },
                      { field: 'Delay', displayName: 'Задержка', width: 200 }],
               };

               $http.get('MPTs/GetMPTsData').success(function (data) {
                   $scope.uiGrid.data = data;
               });

                 //$scope.$on('mptChanged', function (event, args) {
                 //    var data = $scope.gridOptions.data;

                 //    for (item in data) {
                 //        if (args.UID === data[i].UID) {
                 //            data[item].StateIcon = args.StateIcon;
                 //            $scope.$apply();
                 //            break;
                 //        }
                 //    }
                 //});

                $scope.mptClick = function (mpt) {
                    var modalInstance = $uibModal.open({
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

                $scope.$on('mptChanged', function (event, args) {
                    ChangeMPT(args);
                    $scope.$apply();
                })

           }]);
}());