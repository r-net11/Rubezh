(function () {
    'use strict';

    angular.module("gkApp").controller("alarmGroupsCtrl",
        ['$scope', '$http',
            function ($scope, $http) {
                $http.get('Alarms/GetAlarmGroups').success(function (data, status, headers, config) {
                    $scope.model = data;
                });
            }
        ]);
}());
