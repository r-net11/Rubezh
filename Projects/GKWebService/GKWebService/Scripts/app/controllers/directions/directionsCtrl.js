(function () {

    'use strict';

    var app = angular.module('canvasApp.controllers').controller('directionsCtrl',
        ['$scope', '$http', '$uibModal', 'signalrDirectionsService',
        function ($scope, $http, $uibModal, signalrDirectionsService) {
            $scope.gridOptions = {
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                columnDefs: [
                    { field: 'No', enableColumnResizing: false, displayName: '№', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" height="16" width="16" src="/Content/Image/Icon/GK/Blue_Direction.png" />{{row.entity[col.field]}}</div>' },
                    { field: 'Name', minWidth: 210, width: 310, displayName: 'Направление', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.directionClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" />{{row.entity[col.field]}}</a></div>' },
                    { field: 'State', displayName: 'Состояние', width: 300 }
                    { field: 'No', displayName: '№', width: '40', cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" />{{row.entity[col.field]}}</div>' },
                    { field: 'Name', displayName: 'Наименование', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.directionClick(row.entity)">{{row.entity[col.field]}}</a></div>' },
                    { field: 'Delay', displayName: 'Задержка', width: '80', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.Delay}}</div>' },
                    { field: 'Hold', displayName: 'Удержание', width: '85', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.Hold}}</div>' },
                    { field: 'DelayRegime', displayName: 'Режим', width: '80', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.DelayRegime}}</div>' },
                    { field: 'Logic', displayName: 'Логика', cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity.Logic}}</div>' }

                ]
            };

            $scope.signalrDirectionsService = signalrDirectionsService;

            $scope.$on('directionChanged', function (event, args) {
                var data = $scope.gridOptions.data;
                for (var i = 0, len = data.length; i < len; i++) {
                    if (args.UID === data[i].UID) {
                        data[i].State = args.State;
                        data[i].StateIcon = args.StateIcon;
                        $scope.$apply();
                        break;
                    }
                }
            });

            $scope.directionClick = function (direction) {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Directions/DirectionDetails',
                    controller: 'directionDetailsCtrl',
                    resolve: {
                        direction: function () {
                            return direction;
                        }
                    }
                });
            };

            $http.get('Home/GetDirections').success(function (data, status, headers, config) {
                $scope.gridOptions.data = data.rows;
/*
                $scope.config = {
                    myClick: function (rowid) {
                        var modalInstance = $uibModal.open({
                            animation: false,
                            templateUrl: 'Directions/DirectionDetails',
                            controller: 'directionDetailsCtrl',
                            resolve: {
                                direction: function () {
                                    var direction = null;
                                    for (var i = 0; i < data.rows.length; i++) {
                                        if (data.rows[i].UID === rowid) {
                                            direction = data.rows[i];
                                        }
                                    }
                                    return direction;
                                }
                            }
                        });
                    },
                    datatype: "local",
                    colModel: [
                        { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
                        { label: 'Номер', name: 'No', key: false, hidden: false, sortable: false, formatter: imageFormat },
                        { label: 'Наименование', name: 'Name', hidden: false, sortable: false, formatter: linkFormat },
                        { label: 'Состояние', name: 'State', hidden: false, sortable: false }
                    ],
                    width: jQuery(window).width() - 300,
                    height: jQuery(window).height() - 10,
                    rowNum: 100,
                    viewrecords: true
                };

                $scope.data = data.rows;
*/
});
        }]
    );

}());
