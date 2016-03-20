(function () {

    angular.module('gkApp.controllers').controller('directionDetailsCtrl',
        ['$scope', '$uibModalInstance', '$http', '$state', 'signalrDirectionsService', 'entity', 'authService',
        function ($scope, $uibModalInstance, $http, $state, signalrDirectionsService, entity, authService) {
            $scope.direction = entity;

            $scope.$on('directionChanged', function (event, args) {
                if (args.UID === $scope.direction.UID) {
                    $scope.direction = args;
                    $scope.$apply();
                };
            });

            $scope.SetAutomaticState = function () {
                authService.сonfirm().then(function () {
                    $http.post('Directions/SetAutomaticState', { id: $scope.direction.UID });
                });
            };

            $scope.SetManualState = function () {
                authService.сonfirm().then(function () {
                    $http.post('Directions/SetManualState', { id: $scope.direction.UID });
                });
            };

            $scope.SetIgnoreState = function () {
                authService.сonfirm().then(function () {
                    $http.post('Directions/SetIgnoreState', { id: $scope.direction.UID });
                });
            };

            $scope.TurnOn = function () {
                authService.сonfirm().then(function () {
                    $http.post('Directions/TurnOn', { id: $scope.direction.UID });
                });
            };

            $scope.TurnOnNow = function () {
                authService.сonfirm().then(function () {
                    $http.post('Directions/TurnOnNow', { id: $scope.direction.UID });
                });
            };

            $scope.ForbidStart = function () {
                authService.сonfirm().then(function () {
                    $http.post('Directions/ForbidStart', { id: $scope.direction.UID });
                });
            };

            $scope.TurnOff = function () {
                authService.сonfirm().then(function () {
                    $http.post('Directions/TurnOff', { id: $scope.direction.UID });
                });
            };

            $scope.Show = function () {
                $state.go('directions', { uid: $scope.direction.UID });
            };

            $scope.ShowJournal = function() {
                $state.go('archive', { uid: $scope.direction.UID });
            };

            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );
}());