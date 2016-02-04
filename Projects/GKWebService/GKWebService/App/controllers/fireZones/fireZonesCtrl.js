(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('fireZonesCtrl',
        ['$scope', '$http', '$timeout', '$uibModal', 'signalrFireZonesService', 'broadcastService',
        function ($scope, $http, $timeout, $uibModal, signalrFireZonesService, broadcastService) {
            $scope.gridOptions = {
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableRowSelection: true,
                noUnselect: true,
                columnDefs: [
                    { field: 'No', enableColumnResizing: false, displayName: '№', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" ng-src="{{row.entity.ImageSource}}"/>{{row.entity[col.field]}}</div>' },
                    { field: 'Name', width: 200, displayName: 'Наименование', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.fireZonesClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="{{row.entity.StateIcon}}"/>{{row.entity[col.field]}}</a></div>' },
                    { field: 'Fire1Count', displayName: 'Количество датчиков для перевода в Пожар1:', width: Math.round(($(window).width() - 525) / 2) },
                    { field: 'Fire2Count', displayName: 'Количество датчиков для перевода в Пожар2:', width: Math.round(($(window).width() - 525) / 2) }
                ]
            };

            $scope.$on('fireZonesChanged', function (event, args) {
                for (var i in $scope.gridOptions.data) {
                    if (args.Uid === $scope.gridOptions.data[i].Uid) {
                        var isCurrentRowSelected = $scope.gridApi.grid.rows[i].isSelected;
                        $scope.gridOptions.data[i] = args;
                        $scope.$apply();
                        if (isCurrentRowSelected) {
                            $scope.gridApi.selection.selectRow($scope.gridOptions.data[i]);
                        }
                        break;
                    }
                }
            });

            $scope.fireZonesClick = function (fireZone) {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'FireZones/FireZonesDetails',
                    controller: 'fireZonesDetailsCtrl',
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

            $http.get('FireZones/GetFireZonesData').success(function (data, status, headers, config) {
                $scope.gridOptions.data = data;
                //Выбираем первую строку после загрузки данных
                $timeout(function () {
                    if ($scope.gridApi.selection.selectRow) {
                        $scope.gridApi.selection.selectRow($scope.gridOptions.data[0]);
                    }
                });
            });
           
            $scope.gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;
                //Подписчик на событие изменения выбранной строки
                gridApi.selection.on.rowSelectionChanged($scope, $scope.changeZone);
            };

        }]
    );
}());
