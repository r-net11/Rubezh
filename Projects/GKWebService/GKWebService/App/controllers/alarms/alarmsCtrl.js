(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('alarmsCtrl',
        ['$scope', '$http', '$window', '$state', '$stateParams', 'broadcastService', 'constants', 'dialogService',
        function ($scope, $http, $window, $state, $stateParams, broadcastService, constants, dialogService) {
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
                        field: 'GkEntity.Name', minWidth: 200, width: 310, displayName: 'Объект', enableFiltering: false,
                        cellTemplate: '<div ng-style="!row.isSelected && {\'background-color\': row.entity.AlarmColor}" class="ui-grid-cell-contents">\
                                            <a href="#" ng-click="grid.appScope.objectClick(row.entity)">\
                                                <img style="vertical-align: middle; padding-right: 3px" width="16" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.GkEntity.StateClass}}.png"/>\
                                                <img style="vertical-align: middle; padding-right: 3px" width="16" ng-src="/Content/Image/{{row.entity.GkEntity.ImageSource}}"/>\
                                                {{row.entity.GkEntity.Name}}\
                                            </a>\
                                        </div>'
                    },
                    {
                         field: 'Plans', width: 200, displayName: 'План', enableFiltering: false,
                         cellTemplate: '<div ng-style="!row.isSelected && {\'background-color\': row.entity.AlarmColor}" class="ui-grid-cell-contents">\
                                                <span style="padding-right: 3px" ng-repeat="plan in row.entity.Plans">\
                                                    <img style="vertical-align: middle; padding-right: 2px" width="16" ng-src="/Content/Image/Images/CMap.png"/>\
                                                    <a href="#" ng-click="grid.appScope.planClick(plan)">\
                                                        {{plan.Name}}\
                                                    </a>\
                                                </span>\
                                        </div>'
                    },
                    {
                        field: 'GkEntity.Name', width: 300, displayName: 'Команды', enableFiltering: false,
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

            $scope.term = $stateParams.alarmType;
            $scope.gridOptions.enableFiltering = ($stateParams.alarmType ? true : false);


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
                if (alarm.GkEntity.ObjectType === constants.gkObject.device.type) {
                    $state.go('device', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.zone.type) {
                    $state.go('fireZones', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.guardZone.type) {
                    $state.go('guardZone', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.direction.type) {
                    $state.go('directions', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.mpt.type) {
                    $state.go('MPTs', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.delay.type) {
                    $state.go('delays', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.pumpStation.type) {
                    $state.go('pumpStations', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.door.type) {
                    $state.go('doors', { uid: alarm.GkEntity.UID });
                }
            };

            $scope.journalClick = function (alarm) {
                $state.go('archive', { uid: alarm.GkEntity.UID });
            }

            $scope.showPropertiesClick = function (alarm) {
                if (alarm.GkEntity.ObjectType === constants.gkObject.device.type) {
                    dialogService.showWindow(constants.gkObject.device, alarm.GkEntity);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.zone.type) {
                    dialogService.showWindow(constants.gkObject.zone, alarm.GkEntity);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.guardZone.type) {
                    dialogService.showWindow(constants.gkObject.guardZone, alarm.GkEntity);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.direction.type) {
                    dialogService.showWindow(constants.gkObject.direction, alarm.GkEntity);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.mpt.type) {
                    dialogService.showWindow(constants.gkObject.mpt, alarm.GkEntity);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.delay.type) {
                    dialogService.showWindow(constants.gkObject.delay, alarm.GkEntity);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.pumpStation.type) {
                    dialogService.showWindow(constants.gkObject.pumpStation, alarm.GkEntity);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObject.door.type) {
                    dialogService.showWindow(constants.gkObject.door, alarm.GkEntity);
                }
            };
        }]
    );

}());
