(function () {

    angular.module('gkApp.controllers').controller('delayDetailsCtrl',
		['$scope', '$uibModalInstance', '$http', '$state', 'signalrDelaysService', 'entity', 'authService',
         function ($scope, $uibModalInstance, $http, $state, signalrDelaysService, entity, authService) {
		    $scope.delay = entity;

			$scope.$on('delayChanged', function (event, args) {
			    if (args.UID === $scope.delay.UID) {
					$scope.delay = args;
					$scope.$apply();
				};
			});
			$scope.SetAutomaticState = function () {
			    authService.сonfirm().then(function(options) {
			        $http.post('Delays/SetAutomaticState', { id: $scope.delay.UID }, options);
			    });
			};

			$scope.SetManualState = function () {
			    authService.сonfirm().then(function(options) {
			        $http.post('Delays/SetManualState', { id: $scope.delay.UID }, options);
			    });
			};

			$scope.SetIgnoreState = function () {
			    authService.сonfirm().then(function(options) {
			        $http.post('Delays/SetIgnoreState', { id: $scope.delay.UID }, options);
			    });
			};

			$scope.TurnOn = function () {
			    authService.сonfirm().then(function(options) {
			        $http.post('Delays/TurnOn', { id: $scope.delay.UID }, options);
			    });
			};

			$scope.TurnOnNow = function () {
			    authService.сonfirm().then(function(options) {
			        $http.post('Delays/TurnOnNow', { id: $scope.delay.UID }, options);
			    });
			};

			$scope.TurnOff = function () {
			    authService.сonfirm().then(function(options) {
			        $http.post('Delays/TurnOff', { id: $scope.delay.UID }, options);
			    });
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