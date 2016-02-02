(function () {

    var app = angular.module('gkApp.controllers');
    app.controller('DevicesMPTCtrl', ['$scope', '$mediator', 'signalrMPTsService', function ($scope, $mediator, devices) {

            $mediator.$on('my:changeItem', function (event, data) {
                $scope.data = data;
            });

            $scope.data = devices;
            $scope.uiGrid = {
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                columnDefs:
                  [{ field: 'No', displayName: 'No', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" height="16" width="16" src="/Content/Image/Icon/GK/Blue_Direction.png" />{{row.entity[col.field]}}</div>' },
                   { field: 'Name', displayName: 'Устройство', width: 450, cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.mptClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" /> {{row.entity[col.field]}}</a></div>' }],
            };
        }
    ])

}());