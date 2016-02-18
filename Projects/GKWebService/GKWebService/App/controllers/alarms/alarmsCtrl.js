(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('alarmsCtrl',
        ['$scope', '$http', '$uibModal', '$window', '$state', '$stateParams', 'broadcastService', 'constants',
        function ($scope, $http, $uibModal, $window, $state, $stateParams, broadcastService, constants) {
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
                // TODO: Исправить когда меню переведём на ангулар
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.device) {
                    $state.go('device', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.zone) {
                    $state.go('fireZones', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.guardZone) {
                    $state.go('guardZone', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.direction) {
                    $state.go('directions', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.mpt) {
                    $state.go('MPTs', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.delay) {
                    $state.go('delays', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.pumpStation) {
                    $state.go('pumpStations', { uid: alarm.GkEntity.UID });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.door) {
                    $state.go('doors', { uid: alarm.GkEntity.UID });
                }
                // TODO: Дополнить здесь обработку кликов на объекты при создании новых страниц объектов
            };

            $scope.journalClick = function (alarm) {
                // TODO: Исправить когда меню переведём на ангулар
                $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .archive")[0] }, 'Archive');
                broadcastService.send('showArchive', alarm.GkEntity.UID);
            }

            $scope.showPropertiesClick = function (alarm) {
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.device) {
                    broadcastService.send('showDeviceDetails', alarm.GkEntity.UID);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.zone) {
                    broadcastService.send('showGKZoneDetails', alarm.GkEntity.UID);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.guardZone) {
                    broadcastService.send('showGuardZoneDetails', alarm.GkEntity.UID);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.direction) {
                    var modalInstance = $uibModal.open({
                        animation: false,
                        templateUrl: 'Directions/DirectionDetails',
                        controller: 'directionDetailsCtrl',
                        backdrop: false,
                        size: 'rbzh',
                        resolve: {
                            direction: function () {
                                return alarm.GkEntity;
                            }
                        }
                    });
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.mpt) {
                    broadcastService.send('showMPTDetails', alarm.GkEntity.UID);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.delay) {
                    broadcastService.send('showDelayDetails', alarm.GkEntity.UID);
                }
                if (alarm.GkEntity.ObjectType === constants.gkObjectType.pumpStation) {
                    broadcastService.send('showPumpStationDetails', alarm.GkEntity.UID);
                }
            };
        }]
    );

}());
