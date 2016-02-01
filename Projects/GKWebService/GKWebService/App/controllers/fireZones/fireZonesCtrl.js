(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('fireZonesCtrl',
        ['$scope', '$http', '$timeout', '$uibModal', 'signalrFireZonesService',
        function ($scope, $http, $timeout, $uibModal, signalrFireZonesService) {

            $scope.gridOptions = {
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableRowSelection: true,
                rowTemplate: "<div ng-click=\"grid.appScope.changeZone(row)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell></div>",
                columnDefs: [
                    { field: 'No', enableColumnResizing: false, displayName: '№', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" ng-src="data:image/gif;base64,{{row.entity.ImageSource.Item1}}"/>{{row.entity[col.field]}}</div>' },
                    { field: 'Name', width: 200, displayName: 'Наименование', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.fireZonesClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="{{row.entity.StateIcon}}"/>{{row.entity[col.field]}}</a></div>' },
                    { field: 'Fire1Count', displayName: 'Количество датчиков для перевода в Пожар1:', width: Math.round(($(window).width() - 500) / 2) },
                    { field: 'Fire2Count', displayName: 'Количество датчиков для перевода в Пожар2:', width: Math.round(($(window).width() - 500) / 2) }
                ]
            };

            $scope.$on('fireZonesChanged', function (event, args) {
                for (var i in $scope.gridOptions.data) {
                    if (args.Uid === $scope.gridOptions.data[i].Uid) {
                        $scope.gridOptions.data[i] = args;
                        $scope.$apply();
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
                $scope.$parent.zoneNumber = row.entity.No - 1;
                angular.element(document.getElementById('devices')).scope().main();
            }

            $http.get('FireZones/GetFireZonesData').success(function (data, status, headers, config) {
                $scope.gridOptions.data = data;
                $timeout(function () {
                    if ($scope.gridApi.selection.selectRow) {
                        $scope.gridApi.selection.selectRow($scope.gridOptions.data[0]);
                    }
                });
            });

            $scope.gridOptions.onRegisterApi = function (gridApi) {
                $scope.gridApi = gridApi;
            };

            //Номер активной зоны
            $scope.$parent.zoneNumber = 0;
        }]
    );
}());
