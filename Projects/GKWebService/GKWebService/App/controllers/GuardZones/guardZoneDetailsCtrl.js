(function () {
    angular.module('gkApp.controllers').controller('guardZoneDetailsCtrl',
    ['$scope', '$http', '$uibModalInstance', '$state', 'signalrGuardZonesService', 'entity', 'authService',
    function ($scope, $http, $uibModalInstance, $state, signalrGuardZonesService, entity, authService) {

        $scope.guardZone = entity;
        $scope.$on('guardZoneChanged', function (event, args) {
            if (args.UID === $scope.guardZone.UID) {
                $scope.guardZone = args;
                $scope.$apply();
            };
        });

        $scope.SetAutomaticState = function () {
            authService.сonfirm().then(function (options) {
                $http.post('GuardZones/SetAutomaticState', { id: $scope.guardZone.UID }, options);
            });
        }

        $scope.SetManualState = function () {
            authService.сonfirm().then(function (options) {
                $http.post('GuardZones/SetManualState', { id: $scope.guardZone.UID }, options);
            });
        };

        $scope.SetIgnoreState = function () {
            authService.сonfirm().then(function (options) {
                $http.post('GuardZones/SetIgnoreState', { id: $scope.guardZone.UID }, options);
            });
        };

        $scope.TurnOn = function () {
            authService.сonfirm().then(function (options) {
                $http.post('GuardZones/TurnOn', { id: $scope.guardZone.UID }, options);
            });
        };

        $scope.TurnOnNow = function () {
            authService.сonfirm().then(function (options) {
                $http.post('GuardZones/TurnOnNow', { id: $scope.guardZone.UID }, options);
            });
        };

        $scope.TurnOff = function () {
            authService.сonfirm().then(function (options) {
                $http.post('GuardZones/TurnOff', { id: $scope.guardZone.UID }, options);
            });
        };

        $scope.TurnOffNow = function () {
            authService.сonfirm().then(function (options) {
                $http.post('GuardZones/TurnOffNow', { id: $scope.guardZone.UID }, options);
            });
        }

        $scope.Reset = function () {
            authService.сonfirm().then(function (options) {
                $http.post('GuardZones/Reset', { id: $scope.guardZone.UID }, options);
            });
        };


        $scope.Show = function () {
            $state.go('guardZone', { uid: $scope.guardZone.UID });
        };

        $scope.ShowJournal = function () {
            $state.go('archive', { uid: $scope.guardZone.UID });
        };

        $scope.ok = function () {
            $uibModalInstance.close();
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }]);
}())