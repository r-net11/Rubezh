(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('directionsCtrl',
        ['$scope', '$http', '$uibModal', '$document', '$timeout', '$stateParams', 'signalrDirectionsService', 'dialogService', 'constants',
        function ($scope, $http, $uibModal, $document, $timeout, $stateParams, signalrDirectionsService, dialogService, constants) {
            $scope.gridOptions = {
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                onRegisterApi: function (gridApi) {
                    $scope.gridApi = gridApi;
                },
                columnDefs: [
                    { field: 'No', enableColumnResizing: false, displayName: '№', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" height="16" width="16" src="/Content/Image/Icon/GK/Blue_Direction.png" />{{row.entity[col.field]}}</div>' },
                    { field: 'Name', minWidth: 210, width: 310, displayName: 'Направление', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.directionClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png"/>{{row.entity[col.field]}}</a></div>' },
                    { field: 'Delay', displayName: 'Задержка', width: '80', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.Delay}}</div>' },
                    { field: 'Hold', displayName: 'Удержание', width: '85', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.Hold}}</div>' },
                    { field: 'DelayRegime', displayName: 'Режим', width: '80', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.DelayRegime}}</div>' },
                    { field: 'Logic', displayName: 'Логика', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.Logic}}</div>' }

                ]
            };

            $scope.gridStyle = function () {
            	var ctrlHeight = window.innerHeight - 100;
            	return "height:" + ctrlHeight + "px";
            }();


            $scope.$on('directionChanged', function (event, args) {
                var data = $scope.gridOptions.data;
                for (var i = 0, len = data.length; i < len; i++) {
                    if (args.UID === data[i].UID) {
                        data[i] = args;
                        $scope.$apply();
                        break;
                    }
                }
            });

            $scope.directionClick = function (direction) {
                dialogService.showWindow(constants.gkObject.direction, direction);
            };

            $http.get('Directions/GetDirections').then(
                function (response) {
                    $scope.gridOptions.data = response.data;
                    if ($stateParams.uid) {
                        $timeout(function() { $scope.selectRowById($stateParams.uid) });
                    }
                },
                function (response) {
                    // TODO: Нужно реализовать общее окно для отображения ошибок
                    alert(response.data.errorText);
                }
            );

            $scope.selectRowById = function(uid) {
                for (var i = 0; i < $scope.gridOptions.data.length; i++) {
                    if ($scope.gridOptions.data[i].UID === uid) {
                        $scope.gridApi.selection.selectRow($scope.gridOptions.data[i]);
                        $scope.gridApi.core.scrollTo($scope.gridOptions.data[i], $scope.gridOptions.columnDefs[0]);
                        break;
                    }
                }
            }
        }]
    );

}());
