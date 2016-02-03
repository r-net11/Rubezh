(function () {
    'use strict';

    angular.module("gkApp").controller("alarmGroupsCtrl",
        ['$scope', '$http', 'signalrAlarmsService',
            function ($scope, $http, signalrAlarmsService) {
                $http.get('Alarms/GetAlarmGroups').success(function (data, status, headers, config) {
                    $scope.model = data;
                });

                $scope.$on('alarmsChanged', function (event, args) {
                    $scope.model = args.groups;
                    $scope.$apply();
                });
            }
        ]);
}());
