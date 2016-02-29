(function () {

    angular.module('gkApp.controllers').controller('mainIconsCtrl',
        ['$scope', 'authService',
        function ($scope, authService) {
            $scope.logOut = function () {
                authService.logOut();
            };
        }]
    );
}());