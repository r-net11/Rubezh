﻿(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('alarmsCtrl',
        ['$scope', '$http', '$uibModal', '$window',
        function ($scope, $http, $uibModal, $window) {
            $scope.gridOptions = {
                enableFiltering: false,
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
                                            <a href="#" style="padding-right: 3px" ng-show="row.entity.CanReset" ng-click="grid.appScope.resetAlarmClick(row.entity)">\
                                                Сбросить\
                                            </a>\
                                            <a href="#" style="padding-right: 3px" ng-show="row.entity.CanResetIgnore" ng-click="grid.appScope.resetAlarmIgnoreClick(row.entity)">\
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
                $scope.model = args.alarms;
                $scope.gridOptions.data = $scope.model.Alarms;
                $scope.$apply();
            });

            $scope.$on('alarmsShowClick', function (event, args) {
                // TODO: Исправить когда меню переведём на ангулар
                $window.app.Menu.PageClick(null, { currentTarget: angular.element("#menuState")[0] }, 'State');

                $scope.gridOptions.enableFiltering = true;
                $scope.term = args;
                $scope.gridApi.grid.refresh();
            });

            $scope.menuStateClick = function() {
                $scope.gridOptions.enableFiltering = false;
                $scope.gridApi.grid.refresh();
            };

            $http.get('Alarms/GetAlarms').success(function (data, status, headers, config) {
                $scope.model = data;
                $scope.gridOptions.data = $scope.model.Alarms;
            });

            $scope.resetClick = function() {
                $http.post('Alarms/ResetAll');
            };

            $scope.resetAlarmClick = function (alarm) {
                $http.post('Alarms/Reset', { alarm: alarm });
            };

            $scope.resetAlarmIgnoreClick = function (alarm) {
                $http.post('Alarms/ResetIgnore', { alarm: alarm });
            };

            $scope.resetIgnoreAll = function () {
                $http.post('Alarms/ResetIgnoreAll');
            };

            $scope.objectClick = function(alarm) {
                // TODO: Исправить когда меню переведём на ангулар
                if (alarm.GkBaseEntityObjectType === 0) {
                    $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .device")[0] }, 'Device');
                }
                if (alarm.GkBaseEntityObjectType === 1) {
                    $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .zone")[0] }, 'FireZones');
                }
            };
        }]
    );

}());
