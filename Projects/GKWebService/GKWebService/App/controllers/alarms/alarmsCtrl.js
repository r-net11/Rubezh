(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('alarmsCtrl',
        ['$scope', '$http', '$uibModal',
        function ($scope, $http, $uibModal) {
            $scope.gridOptions = {
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                columnDefs: [
                    {
                        field: 'AlarmName', displayName: 'Состояние', width: 300,
                            cellTemplate: '<div class="ui-grid-cell-contents">\
                                           <img style="vertical-align: middle; padding-right: 3px" width="16" \
                                                ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.AlarmImageSource}}.png" />\
                                           {{row.entity[col.field]}}</div>'
                    },
                    {
                        field: 'ObjectName', minWidth: 200, width: 310, displayName: 'Объект',
                        cellTemplate: '<div class="ui-grid-cell-contents">\
                                            <a href="#" ng-click="grid.appScope.objectClick(row.entity)">\
                                                <img style="vertical-align: middle; padding-right: 3px" width="16" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.ObjectStateClass}}.png"/>\
                                                <img style="vertical-align: middle; padding-right: 3px" width="16" ng-src="/Content/Image/{{row.entity.ObjectImageSource}}.png"/>\
                                                {{row.entity[col.field]}}\
                                            </a>\
                                        </div>'
                    },
                    { field: 'Plans', width: 200, displayName: 'План' },
                    {
                        field: 'ObjectName', width: 200, displayName: 'Команды',
                        cellTemplate: '<div class="ui-grid-cell-contents">\
                                            <a href="#" style="padding-right: 3px" ng-click="grid.appScope.journalClick(row.entity)">\
                                                Журнал\
                                            </a>\
                                            <a href="#" style="padding-right: 3px" ng-show="row.entity.CanReset" ng-click="grid.appScope.resetClick(row.entity)">\
                                                Сбросить\
                                            </a>\
                                            <a href="#" style="padding-right: 3px" ng-show="row.entity.CanResetIgnore" ng-click="grid.appScope.resetIgnoreClick(row.entity)">\
                                                Снять отключение\
                                            </a>\
                                            <a href="#" style="padding-right: 3px" ng-show="row.entity.CanTurnOnAutomatic" ng-click="grid.appScope.turnOnAutomaticClick(row.entity)">\
                                                Включить автоматику\
                                            </a>\
                                            <a href="#" style="padding-right: 3px" ng-show="row.entity.CanShowProperties" ng-click="grid.appScope.showPropertiesClick(row.entity)">\
                                                Свойства\
                                            </a>\
                                        </div>'
                    }
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
