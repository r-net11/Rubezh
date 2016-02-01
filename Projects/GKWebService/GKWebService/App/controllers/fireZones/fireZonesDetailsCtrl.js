(function () {

    angular.module('gkApp.controllers').controller('fireZonesDetailsCtrl',
        function ($scope, $uibModalInstance, $http, fireZone) {
            $scope.fireZone = fireZone;

            $scope.$on('fireZonesChanged', function (event, args) {
                if (args.Uid === $scope.fireZone.Uid) {
                    $scope.fireZone = args;
                    $scope.$apply();
                };
            });

            $scope.TurnOn = function () {
                $http.post('FireZones/TurnOn', { id: $scope.fireZone.Uid });
            };
            
            $scope.TurnOff = function () {
                $http.post('FireZones/TurnOff', { id: $scope.fireZone.Uid });
            };

            $scope.Reset = function () {
                $http.post('FireZones/Reset', { id: $scope.fireZone.Uid });
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