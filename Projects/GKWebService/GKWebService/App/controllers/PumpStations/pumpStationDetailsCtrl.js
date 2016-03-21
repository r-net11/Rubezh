(function () {

    angular.module('gkApp.controllers').controller('pumpStationDetailsCtrl',
    ['$scope', '$uibModalInstance', '$http', '$state', 'signalrPumpStatoinsService', 'entity', 'authService',
    function ($scope, $uibModalInstance, $http, $state, signalrPumpStatoinsService, entity, authService) {
        $scope.pumpStation = entity;

        $scope.$on('pumpStationsChanged', function (event, args) {
            if (args.UID === $scope.pumpStation.UID) {
                $scope.pumpStation = args;
                $scope.$apply();
            };
        });

        $scope.SetAutomaticState = function () {
            authService.сonfirm().then(function (options) {
                $http.post('PumpStations/SetAutomaticState', { id: $scope.pumpStation.UID }, options);
            });
        };

        $scope.SetManualState = function () {
            authService.сonfirm().then(function (options) {
                $http.post('PumpStations/SetManualState', { id: $scope.pumpStation.UID }, options);
            });
        };

        $scope.SetIgnoreState = function () {
            authService.сonfirm().then(function (options) {
                $http.post('PumpStations/SetIgnoreState', { id: $scope.pumpStation.UID }, options);
            });
        };

        $scope.TurnOn = function () {
            authService.сonfirm().then(function (options) {
                $http.post('PumpStations/TurnOn', { id: $scope.pumpStation.UID }, options);
            });
        };

        $scope.TurnOnNow = function () {
            authService.сonfirm().then(function (options) {
                $http.post('PumpStations/TurnOnNow', { id: $scope.pumpStation.UID }, options);
            });
        };

        $scope.ForbidStart = function () {
            authService.сonfirm().then(function (options) {
                $http.post('PumpStations/ForbidStart', { id: $scope.pumpStation.UID }, options);
            });
        };

        $scope.TurnOff = function () {
            authService.сonfirm().then(function (options) {
                $http.post('PumpStations/TurnOff', { id: $scope.pumpStation.UID }, options);
            });
        };

        $scope.Show = function () {
            $state.go('pumpStations', { uid: $scope.pumpStation.UID });
        };

        $scope.ShowJournal = function () {
            $state.go('archive', { uid: $scope.pumpStation.UID });
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