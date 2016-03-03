(function () {

    angular.module('gkApp.controllers').controller('delayDetailsCtrl',
		['$scope', '$uibModalInstance', '$http', '$state', 'signalrDelaysService', 'entity',
        function ($scope, $uibModalInstance, $http, $state, signalrDelaysService, entity) {
		    $scope.delay = entity;

			$scope.$on('delayChanged', function (event, args) {
			    if (args.UID === $scope.delay.UID) {
					$scope.delay = args;
					$scope.$apply();
				};
			});
			$scope.SetAutomaticState = function () {
			    $http.post('Delays/SetAutomaticState', { id: $scope.delay.UID });
			};

			$scope.SetManualState = function () {
			    $http.post('Delays/SetManualState', { id: $scope.delay.UID });
			};

			$scope.SetIgnoreState = function () {
			    $http.post('Delays/SetIgnoreState', { id: $scope.delay.UID });
			};

			$scope.TurnOn = function () {
			    $http.post('Delays/TurnOn', { id: $scope.delay.UID });
			};

			$scope.TurnOnNow = function () {
			    $http.post('Delays/TurnOnNow', { id: $scope.delay.UID });
			};

			$scope.TurnOff = function () {
			    $http.post('Delays/TurnOff', { id: $scope.delay.UID });
			};
			$scope.Show = function () {
			    $state.go('delays', { uid: $scope.delay.UID });
			};

			$scope.ShowJournal = function () {
			    $state.go('archive', { uid: $scope.delay.UID });
			};

			$scope.cancel = function () {
			    $uibModalInstance.dismiss('cancel');
			};
        }]
	);
}());