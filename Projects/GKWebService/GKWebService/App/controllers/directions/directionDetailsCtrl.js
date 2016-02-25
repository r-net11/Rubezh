﻿(function () {

    angular.module('gkApp.controllers').controller('directionDetailsCtrl',
        ['$scope', '$uibModalInstance', '$http', '$state', 'signalrDirectionsService', 'entity',
        function ($scope, $uibModalInstance, $http, $state, signalrDirectionsService, entity) {
            $scope.direction = entity;

            $scope.$on('directionChanged', function (event, args) {
                if (args.UID === $scope.direction.UID) {
                    $scope.direction = args;
                    $scope.$apply();
                };
            });

            $scope.SetAutomaticState = function () {
                $http.post('Directions/SetAutomaticState', { id: $scope.direction.UID });
            };

            $scope.SetManualState = function () {
                $http.post('Directions/SetManualState', { id: $scope.direction.UID });
            };

            $scope.SetIgnoreState = function () {
                $http.post('Directions/SetIgnoreState', { id: $scope.direction.UID });
            };

            $scope.TurnOn = function () {
                $http.post('Directions/TurnOn', { id: $scope.direction.UID });
            };

            $scope.TurnOnNow = function () {
                $http.post('Directions/TurnOnNow', { id: $scope.direction.UID });
            };

            $scope.ForbidStart = function () {
                $http.post('Directions/ForbidStart', { id: $scope.direction.UID });
            };

            $scope.TurnOff = function () {
                $http.post('Directions/TurnOff', { id: $scope.direction.UID });
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