(function () {
    angular.module('gkApp.controllers').controller('guardZoneDetailsCtrl',
    ['$scope', '$http', '$uibModalInstance', '$state', 'signalrGuardZonesService', 'entity',
    function ($scope, $http, $uibModalInstance, $state, signalrGuardZonesService, entity) {

        $scope.guardZone = entity;
        $scope.$on('guardZoneChanged', function (event, args) {
            if (args.Uid === $scope.guardZone.Uid) {
                $scope.guardZone = args;
                $scope.$apply();
            };
        });

        $scope.SetAutomaticState = function () {
            $http.post('GuardZones/SetAutomaticState', { id: $scope.guardZone.UID });
        }

        $scope.SetManualState = function () {
            $http.post('GuardZones/SetManualState', { id: $scope.guardZone.UID });
        };

        $scope.SetIgnoreState = function () {
            $http.post('GuardZones/SetIgnoreState', { id: $scope.guardZone.UID });
        };

        $scope.TurnOn = function () {
            $http.post('GuardZones/TurnOn', { id: $scope.guardZone.UID });
        };

        $scope.TurnOnNow = function () {
            $http.post('GuardZones/TurnOnNow', { id: $scope.guardZone.UID });
        };

        $scope.TurnOff = function () {
            $http.post('GuardZones/TurnOff', { id: $scope.guardZone.UID });
        };

        $scope.Reset = function () {
            $http.post('GuardZones/Reset', { id: $scope.guardZone.UID });
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