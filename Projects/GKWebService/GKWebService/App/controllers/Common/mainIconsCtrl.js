(function () {

    angular.module('gkApp.controllers').controller('mainIconsCtrl',
        ['$scope', 'authService', 'signalrSoundsService',
	function ($scope, authService, signalrSoundsService) {
            $scope.logOut = function () {
            	authService.logOut();
            };

            $scope.isSound = signalrSoundsService.getIsSound();

            signalrSoundsService.onSoundPlay(function () {
            	signalrSoundsService.setIsSound(true);
            	$scope.isSound = true;
            });

            $scope.soundClick = function () {
            	signalrSoundsService.setIsSound(!$scope.isSound)
            	$scope.isSound = !$scope.isSound;
            }
        }]
    );
}());