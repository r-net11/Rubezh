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

            $scope.ResetIgnore = function () {
                $http.post('FireZones/ResetIgnore', { id: $scope.fireZone.Uid });
            };
            
            $scope.SetIgnore = function () {
                $http.post('FireZones/SetIgnore', { id: $scope.fireZone.Uid });
            };

            $scope.ResetFire = function () {
                $http.post('FireZones/ResetFire', { id: $scope.fireZone.Uid });
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