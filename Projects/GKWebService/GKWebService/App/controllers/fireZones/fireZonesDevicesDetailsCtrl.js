(function () {

    angular.module('gkApp.controllers').controller('fireZonesDevicesDetailsCtrl',
        function ($scope, $uibModalInstance, $http, fireZone) {
            $scope.fireZone = fireZone;

            $scope.$on('fireZonesDeviceChanged', function (event, args) {
                if (args.Uid === $scope.fireZoneDevice.Uid) {
                    $scope.fireZoneDevice = args;
                    $scope.$apply();
                };
            });

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