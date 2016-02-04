(function () {
    'use strict';

    angular.module("gkApp").controller("alarmsCtrl",
        ['$scope', '$http', 'signalrAlarmsService',
            function ($scope, $http, signalrAlarmsService) {
                $http.get('Alarms/GetAlarms').success(function (data, status, headers, config) {
                    $scope.model = data;
                });

                $scope.$on('alarmsChanged', function (event, args) {
                    $scope.model = args.alarms;
                    $scope.$apply();
                });
            }
        ]);
}());
