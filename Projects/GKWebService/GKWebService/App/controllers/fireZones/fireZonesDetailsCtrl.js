(function () {

    angular.module('gkApp.controllers').controller('fireZonesDetailsCtrl',
        ['$scope', '$uibModalInstance', '$http', '$state', 'signalrFireZonesService', 'entity'
        function ($scope, $uibModalInstance, $http, $state, signalrFireZonesService, entity) {
            $scope.fireZone = entity;

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
                $state.go('fireZones', { uid: $scope.fireZone.Uid });
            };

            $scope.ShowJournal = function () {
                $state.go('archive', { uid: $scope.fireZone.Uid });
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