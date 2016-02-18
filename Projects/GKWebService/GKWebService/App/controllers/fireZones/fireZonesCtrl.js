(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('fireZonesCtrl',
        ['$scope', '$http', '$timeout', '$uibModal', '$stateParams', 'uiGridConstants', 'signalrFireZonesService', 'broadcastService',
        function ($scope, $http, $timeout, $uibModal, $stateParams, uiGridConstants, signalrFireZonesService, broadcastService) {
	        var fireCountWidth = Math.round(($(window).width() - 525) / 2);
            $scope.gridOptions = {
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableRowSelection: true,
                noUnselect: true,
                enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
                columnDefs: [
                    { field: 'No', enableColumnResizing: false, displayName: '№', width: 60, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/{{row.entity.ImageSource}}"/>{{row.entity[col.field]}}</div>' },
                    { field: 'Name', width: 190, displayName: 'Наименование', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.fireZonesClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png"/>{{row.entity[col.field]}}</a></div>' },
                    { field: 'Fire1Count', displayName: 'Количество датчиков для перевода в Пожар1:', width: fireCountWidth },
                    { field: 'Fire2Count', displayName: 'Количество датчиков для перевода в Пожар2:', width: fireCountWidth }
                ]
            };

            $scope.$on('fireZonesChanged', function (event, args) {
                for (var i in $scope.gridOptions.data) {
                    if (args.Uid === $scope.gridOptions.data[i].Uid) {
                        $scope.gridOptions.data[i].CanResetIgnore = args.CanResetIgnore;
                        $scope.gridOptions.data[i].CanSetIgnore = args.CanSetIgnore;
                        $scope.gridOptions.data[i].CanResetFire = args.CanResetFire;
                        $scope.gridOptions.data[i].StateColor = args.StateColor;
                        $scope.gridOptions.data[i].StateIcon = args.StateIcon;
                        $scope.gridOptions.data[i].StateMessage = args.StateMessage;
                        $scope.$apply();
                        break;
                    }
                }
            });

            $scope.fireZonesClick = function (fireZone) {
					$uibModal.open({
                    animation: false,
                    size: 'rbzh',
                    templateUrl: 'FireZones/FireZonesDetails',
                    controller: 'fireZonesDetailsCtrl',
                    backdrop: false,
                    resolve: {
                        fireZone: function () {
                            return fireZone;
                        }
                    }
                });
            };

            $scope.changeZone = function (row) {
                broadcastService.send('selectedZoneChanged', row.entity.Uid);
            }

            $http.get('FireZones/GetFireZonesData').success(function (data) {
                $scope.gridOptions.data = data;
                //Выбираем первую строку после загрузки данных
                $timeout(function () {
                    if ($stateParams.uid) {
                        $scope.selectRowById($stateParams.uid);
                    } else {
                        if ($scope.gridApi.selection.selectRow) {
                            $scope.gridApi.selection.selectRow($scope.gridOptions.data[0]);
                        }
                    }
                });
            });
           
            $scope.gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;
                //Подписчик на событие изменения выбранной строки
                gridApi.selection.on.rowSelectionChanged($scope, $scope.changeZone);
            };

            $scope.selectRowById = function (uid) {
                for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                    if ($scope.gridOptions.data[i].Uid === uid) {
                        $scope.gridApi.selection.selectRow($scope.gridOptions.data[i]);
                        break;
                    }
                }
            }

            $scope.$on('showGKZoneDetails', function (event, args) {
                for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                    if ($scope.gridOptions.data[i].Uid === args) {
                        $scope.fireZonesClick($scope.gridOptions.data[i]);
                        break;
                    }
                }
            });
        }]
    );
}());
