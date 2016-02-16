(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('navCtrl',
        ['$scope', '$location', '$state', '$rootScope', '$window',
        function ($scope, $location, $state, $rootScope, $window) {
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
                    if (toState.name === 'state') {
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
