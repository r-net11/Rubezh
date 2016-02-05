(function () {

    angular.module('gkApp.controllers').controller('fireZonesDevicesDetailsCtrl',
        function ($scope, $uibModalInstance, $http, device) {
            $scope.device = device;

            $scope.$on('fireZonesDeviceChanged', function (event, args) {
                if (args.Uid === $scope.device.Uid) {
                    $scope.device = args;
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