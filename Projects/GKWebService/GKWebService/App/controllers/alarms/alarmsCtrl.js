(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('directionsCtrl',
        ['$scope', '$http', '$uibModal',
        function ($scope, $http, $uibModal) {
            $scope.gridOptions = {
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                columnDefs: [
                    { field: 'AlarmName', displayName: 'Состояние', width: 300, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" height="16" width="16" src="/Content/Image/Icon/GKStateIcons/{{row.entity.AlarmImageSource}}.png" />{{row.entity[col.field]}}</div>' },
                    { field: 'ObjectName', minWidth: 200, width: 310, displayName: 'Объект', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.objectClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.ObjectImageSource}}.png"/>{{row.entity[col.field]}}</a></div>' }
                ]
            };

            $scope.$on('alarmsChanged', function (event, args) {
                $scope.gridOptions.data = args.alarms.Alarms;
                $scope.$apply();
            });

            $http.get('Alarms/GetAlarms').success(function (data, status, headers, config) {
                $scope.gridOptions.data = data.Alarms;
            });
        }]
    );

}());
