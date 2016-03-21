(function () {
    'use strict';

    angular.module("gkApp").controller("alarmGroupsCtrl",
        ['$scope', '$http', '$sce', '$state', 'signalrAlarmsService', 'broadcastService', 'authService',
            function ($scope, $http, $sce, $state, signalrAlarmsService, broadcastService, authService) {
            	$scope.resetTooltip = $sce.trustAsHtml('<span class="tooltipLine" style="background-image: url(/Content/Image/Images/BReset.png);">Сбросить все</span>');
                
                $http.get('Alarms/GetAlarmGroups').success(function (data, status, headers, config) {
                    $scope.initData(data);
                    $scope.model = data;
                });

                $scope.$on('alarmsChanged', function (event, args) {
                    $scope.initData(args.groups);
                    $scope.model = args.groups;
                    $scope.$apply();
                });

                $scope.isAuth = function() {
                    return authService.authentication.isAuth;
                };

                $scope.ShowClick = function (group) {
                    $state.go('state', { alarmType: group.AlarmType });
                    //broadcastService.send('alarmsShowClick', group.AlarmType);
                };

                $scope.resetClick = function () {
                    authService.сonfirm().then(function(options) {
                        $http.post('Alarms/ResetAll', {}, options);
                    });
                };

                $scope.initData = function(data) {
                    angular.forEach(data.AlarmGroups, function (group) {
                        group.tooltip = $scope.getGroupTooltip(group);
                    });
                };

                $scope.getGroupTooltip = function (group) {
                	var result = '<span class="tooltipLine" style="background-image: url(/Content/Image/Icon/GKStateIcons/' + group.AlarmImageSource + '.png)">' + group.AlarmName + '</span>';
                    angular.forEach(group.Alarms, function (alarm) {
                    	result += '<span class="tooltipLine"style="background-image: url(/Content/Image/' + alarm.ObjectImageSource + ')">' + alarm.ObjectName + '</span>';
                    });
                    return $sce.trustAsHtml(result);
                };
            }
        ]);
}());
