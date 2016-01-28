﻿(function () {

	angular.module('canvasApp.controllers').controller('delayDetailsCtrl',
		function ($scope, $uibModalInstance, $http, delay) {
			$scope.delay = delay;

			$scope.$on('delayChanged', function (event, args) {
				if (args.Uid === $scope.delay.Uid) {
					$scope.delay = args;
					$scope.$apply();
				};
			});
			$scope.SetAutomaticState = function () {
				$http.post('Delays/SetAutomaticState', { id: $scope.delay.Uid });
			};

			$scope.SetManualState = function () {
				$http.post('Delays/SetManualState', { id: $scope.delay.Uid });
			};

			$scope.SetIgnoreState = function () {
				$http.post('Delays/SetIgnoreState', { id: $scope.delay.Uid });
			};

			$scope.TurnOn = function () {
				$http.post('Delays/TurnOn', { id: $scope.delay.Uid });
			};

			$scope.TurnOnNow = function () {
				$http.post('Delays/TurnOnNow', { id: $scope.delay.Uid });
			};

			$scope.TurnOff = function () {
				$http.post('Delays/TurnOff', { id: $scope.delay.Uid });
			};
			$scope.Show = function () {

			};

			$scope.ShowJournal = function () {

			};
		}
	);
}());