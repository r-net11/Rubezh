(function () {

    angular.module('gkApp.controllers').controller('pumpStationDetailsCtrl', function ($scope, $uibModalInstance, $http, entity) {
        $scope.pumpStation = entity;

        $scope.$on('pumpStationsChanged', function (event, args) {
            if (args.UID === $scope.pumpStation.UID) {
                $scope.pumpStation = args;
                $scope.$apply();
            };
        });

        $scope.SetAutomaticState = function () {
            $http.post('PumpStations/SetAutomaticState', { id: $scope.pumpStation.UID });
        };

        $scope.SetManualState = function () {
            $http.post('PumpStations/SetManualState', { id: $scope.pumpStation.UID });
        };

        $scope.SetIgnoreState = function () {
            $http.post('PumpStations/SetIgnoreState', { id: $scope.pumpStation.UID });
        };

        $scope.TurnOn = function () {
            $http.post('PumpStations/TurnOn', { id: $scope.pumpStation.UID });
        };

        $scope.TurnOnNow = function () {
            $http.post('PumpStations/TurnOnNow', { id: $scope.pumpStation.UID });
        };

        $scope.ForbidStart = function () {
            $http.post('PumpStations/ForbidStart', { id: $scope.pumpStation.UID });
        };

        $scope.TurnOff = function () {
            $http.post('PumpStations/TurnOff', { id: $scope.pumpStation.UID });
        };

        $scope.Show = function () {

        };

        $scope.ShowJournal = function () {

        };

        $scope.ok = function () {
            $uibModalInstance.close();
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
    );
}());