(function () {
    'use strict';

    angular.module("gkApp").controller("alarmGroupsCtrl",
        ['$scope', '$http', 'signalrAlarmsService', 'broadcastService',
            function ($scope, $http, signalrAlarmsService, broadcastService) {
                $http.get('Alarms/GetAlarmGroups').success(function (data, status, headers, config) {
                    $scope.model = data;
                });

                $scope.$on('alarmsChanged', function (event, args) {
                    $scope.model = args.groups;
                    $scope.$apply();
                });

                $scope.ShowClick = function(group) {
                    broadcastService.send('alarmsShowClick', group.AlarmType);
                };

                $scope.resetClick = function () {
                    $http.post('Alarms/ResetAll');
                };
            }
        ]);
}());
