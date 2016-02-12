(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('alarmsCtrl',
        ['$scope', '$http', '$uibModal', '$window', 'broadcastService', 'constants',
        function ($scope, $http, $uibModal, $window, broadcastService, constants) {
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
                                                <span style="padding-right: 3px" ng-repeat="plan in row.entity.Plans">\
                                                    <img style="vertical-align: middle; padding-right: 2px" width="16" ng-src="/Content/Image/Images/CMap.png"/>\
                                                    <a href="#" ng-click="grid.appScope.planClick(plan)">\
                                                        {{plan.Name}}\
                                                    </a>\
                                                </span>\
                                        </div>'
                    },
                    {
                        field: 'ObjectName', width: 300, displayName: 'Команды', enableFiltering: false,
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
                angular.element(".menu .group-control").parent().addClass('clicked');
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
                angular.element(".menu .group-control").parent().addClass('clicked');
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.device) {
                    $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .device")[0] }, 'Device');
                    broadcastService.send('showGKDevice', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.zone) {
                    $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .zone")[0] }, 'FireZones');
                    broadcastService.send('showGKZone', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.guardZone) {
                    $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .guardZone")[0] }, 'GuardZone');
                    broadcastService.send('showGKGuardZone', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.direction) {
                    $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .direction")[0] }, 'Directions');
                    broadcastService.send('showGKDirection', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.mpt) {
                    $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .MPTs")[0] }, 'MPTs');
                    broadcastService.send('showGKMPT', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.delay) {
                    $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .delays")[0] }, 'Delays');
                    broadcastService.send('showGKDelay', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.pumpStation) {
                    $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .pumpStations")[0] }, 'PumpStations');
                    broadcastService.send('showGKPumpStation', alarm.GkBaseEntityUID);
                }
                // TODO: Дополнить здесь обработку кликов на объекты при создании новых страниц объектов
            };

            $scope.journalClick = function (alarm) {
                // TODO: Исправить когда меню переведём на ангулар
                $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .archive")[0] }, 'Archive');
                broadcastService.send('showArchive', alarm.GkBaseEntityUID);
            }

            $scope.showPropertiesClick = function (alarm) {
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.device) {
                    broadcastService.send('showDeviceDetails', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.zone) {
                    broadcastService.send('showGKZoneDetails', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.guardZone) {
                    broadcastService.send('showGuardZoneDetails', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.direction) {
                    broadcastService.send('showDirectionDetails', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.mpt) {
                    broadcastService.send('showMPTDetails', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.delay) {
                    broadcastService.send('showDelayDetails', alarm.GkBaseEntityUID);
                }
                if (alarm.GkBaseEntityObjectType === constants.gkObjectType.pumpStation) {
                    broadcastService.send('showPumpStationDetails', alarm.GkBaseEntityUID);
                }
            };
        }]
    );

}());
