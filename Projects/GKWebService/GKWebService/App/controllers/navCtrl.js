(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('navCtrl',
        ['$scope', '$location', '$state', '$rootScope',
        function ($scope, $location, $state, $rootScope) {
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
                });
        }]
    );

}());
