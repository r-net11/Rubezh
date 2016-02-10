(function () {
    'use strict';

    angular.module("gkApp").controller("alarmGroupsCtrl",
        ['$scope', '$http', '$sce', 'signalrAlarmsService', 'broadcastService',
            function ($scope, $http, $sce, signalrAlarmsService, broadcastService) {
                $scope.resetTooltip = $sce.trustAsHtml('<img width="16" src="/Content/Image/Images/BReset.png" /> <span>Сбросить все</span>');
                
                $http.get('Alarms/GetAlarmGroups').success(function (data, status, headers, config) {
                    $scope.initData(data);
                    $scope.model = data;
                });

                $scope.$on('alarmsChanged', function (event, args) {
                    $scope.initData(args.groups);
                    $scope.model = args.groups;
                    $scope.$apply();
                });

                $scope.ShowClick = function(group) {
                    broadcastService.send('alarmsShowClick', group.AlarmType);
                };

                $scope.resetClick = function () {
                    $http.post('Alarms/ResetAll');
                };

                $scope.initData = function(data) {
                    angular.forEach(data.AlarmGroups, function (group) {
                        group.tooltip = $scope.getGroupTooltip(group);
                    });
                };

                $scope.getGroupTooltip = function (group) {
                    var result = '<img width="16" src="/Content/Image/Icon/GKStateIcons/' + group.AlarmImageSource + '.png"/> <span>' + group.AlarmName + '</span><br/>';
                    angular.forEach(group.Alarms, function (alarm) {
                        result += '<img width="16" src="/Content/Image/' + alarm.ObjectImageSource + '.png"/> <span>' + alarm.ObjectName + '</span><br/>';
                    });
                    return $sce.trustAsHtml(result);
                };
            }
        ]);
}());
