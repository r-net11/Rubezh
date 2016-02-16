(function () {
    angular.module('gkApp').controller('doorDetailsCtrl', function ($scope, $uibModalInstance, $http, door) {

        $scope.door = door;
        $scope.$on('doorChanged', function (event, args) {
            if (args.UID === door.UID) {
                $scope.door = args;
                $scope.$apply();
            };
        });

        $scope.SetAutomaticState = function () {
            $http.post('Doors/SetAutomaticState', { id: $scope.door.UID });
        }

        $scope.SetManualState = function () {
            $http.post('Doors/SetManualState', { id: $scope.door.UID });
        };

        $scope.SetIgnoreState = function () {
            $http.post('Doors/SetIgnoreState', { id: $scope.door.UID });
        };

        $scope.TurnOn = function () {
            $http.post('Doors/TurnOn', { id: $scope.door.UID });
        };

        $scope.TurnOnNow = function () {
            $http.post('Doors/TurnOnNow', { id: $scope.door.UID });
        };

        $scope.TurnOff = function () {
            $http.post('Doors/TurnOff', { id: $scope.door.UID });
        };

        $scope.Reset = function () {
            $http.post('Doors/Reset', { id: $scope.door.UID });
        };

        $scope.SetRegimeNorm = function () {
            $http.post('Doors/SetRegimeNorm', { id: $scope.door.UID });
        };

        $scope.SetRegimeOpen = function () {
            $http.post('Doors/SetRegimeOpen', { id: $scope.door.UID });
        };

        $scope.SetRegimeClose = function () {
            $http.post('Doors/SetRegimeClose', { id: $scope.door.UID });
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
    })
}());