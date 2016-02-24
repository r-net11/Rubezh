(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('navCtrl',
        ['$scope', '$location', '$state', '$rootScope', '$window', 'signalrConfigService',
        function ($scope, $location, $state, $rootScope, $window, signalrConfigService) {
            $scope.pageClick = function(page) {
            };

            $scope.menuStateClick = function (group) {
                $state.go('state', { alarmType: '' });
            };

            $scope.isPageActive = function(page) {
                return $state.includes(page);
            };

            $rootScope.$on('$stateChangeStart',
                function(event, toState, toParams, fromState, fromParams, options) {
                    // Раскрываем узел главного меню "Групповой контроллер" при переходе программно из другого пункта меню
                    if (toState.name === 'state' ||
                        toState.name === 'fireZones' ||
                        toState.name === 'device' ||
                        toState.name === 'params' ||
                        toState.name === 'pumpStations' ||
                        toState.name === 'delays' ||
                        toState.name === 'MPTs' ||
                        toState.name === 'guardZone' ||
                        toState.name === 'doors' ||
                        toState.name === 'directions') {
                        $scope.groupControlClicked = true;
                    }

                    if (toState.name === 'hr') {
                        // TODO: исправить, когда переведём картотеку на ангулар
                        $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .hr")[0] }, 'HR');
                    } else {
                        $window.app.Menu.PageClick(null, { currentTarget: angular.element(".menu .undefined")[0] }, null);
                    }
                });
        }]
    );

}());
