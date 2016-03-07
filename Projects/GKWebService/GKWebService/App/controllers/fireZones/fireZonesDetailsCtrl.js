(function () {

    angular.module('gkApp.controllers').controller('fireZonesDetailsCtrl',
        ['$scope', '$uibModalInstance', '$http', '$state', 'signalrFireZonesService', 'entity',
        function ($scope, $uibModalInstance, $http, $state, signalrFireZonesService, entity) {
            $scope.fireZone = entity;

            $scope.$on('fireZonesChanged', function (event, args) {
                if (args.UID === $scope.fireZone.UID) {
                    $scope.fireZone = args;
                    $scope.$apply();
                };
            });

            $scope.ResetIgnore = function () {
                $http.post('FireZones/ResetIgnore', { id: $scope.fireZone.UID });
            };
            
            $scope.SetIgnore = function () {
                $http.post('FireZones/SetIgnore', { id: $scope.fireZone.UID });
            };

            $scope.ResetFire = function () {
                $http.post('FireZones/ResetFire', { id: $scope.fireZone.UID });
            };

            $scope.Show = function () {
                $state.go('fireZones', { uid: $scope.fireZone.UID });
            };

            $scope.ShowJournal = function () {
                $state.go('archive', { uid: $scope.fireZone.UID });
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