(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('alarmsCtrl',
        ['$scope', '$http', '$uibModal', '$window',
        function ($scope, $http, $uibModal, $window) {
            $scope.gridOptions = {
                enableFiltering: true,
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                onRegisterApi: function (gridApi) {
                    $scope.gridApi = gridApi;
                },
                columnDefs: [
                    {
                        field: 'AlarmType', enableFiltering: false,
                        filter: {
                            noTerm: true,
                            condition: function (searchTerm, cellValue) {
                                return cellValue == $scope.term;
                            }
                        },
                        visible: false
                    },
                    {
                        field: 'AlarmName', displayName: 'Состояние', width: 300, enableFiltering: false,
                        cellTemplate: '<div ng-style="!row.isSelected && {\'background-color\': row.entity.AlarmColor}" class="ui-grid-cell-contents">\
                                           <img style="vertical-align: middle; padding-right: 3px" width="16" \
                                                ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.AlarmImageSource}}.png" />\
                                           {{row.entity[col.field]}}</div>'
                    },
                    {
                        field: 'ObjectName', minWidth: 200, width: 310, displayName: 'Объект', enableFiltering: false,
                        cellTemplate: '<div ng-style="!row.isSelected && {\'background-color\': row.entity.AlarmColor}" class="ui-grid-cell-contents">\
                                            <a href="#" ng-click="grid.appScope.objectClick(row.entity)">\
                                                <img style="vertical-align: middle; padding-right: 3px" width="16" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.ObjectStateClass}}.png"/>\
                                                <img style="vertical-align: middle; padding-right: 3px" width="16" ng-src="/Content/Image/{{row.entity.ObjectImageSource}}.png"/>\
                                                {{row.entity[col.field]}}\
                                            </a>\
                                        </div>'
                    },
                    {
                         field: 'Plans', width: 200, displayName: 'План', enableFiltering: false,
                         cellTemplate: '<div ng-style="!row.isSelected && {\'background-color\': row.entity.AlarmColor}" class="ui-grid-cell-contents">\
                                                {{row.entity[col.field]}}\
                                        </div>'
                    },
                    {
                        field: 'ObjectName', width: 200, displayName: 'Команды', enableFiltering: false,
                        cellTemplate: '<div ng-style="!row.isSelected && {\'background-color\': row.entity.AlarmColor}" class="ui-grid-cell-contents">\
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

            $scope.$on('alarmsShowClick', function (event, args) {
                $scope.term = args;
                $scope.gridApi.grid.refresh();
            });

            $http.get('Alarms/GetAlarms').success(function (data, status, headers, config) {
                $scope.gridOptions.data = data.Alarms;
            });

            $scope.resetClick = function() {
                $http.post('Alarms/ResetAll');
            };
        }]
    );

}());
