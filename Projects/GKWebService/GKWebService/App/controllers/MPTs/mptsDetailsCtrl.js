(function () {

    angular.module('gkApp.controllers').controller('mptsDetailsCtrl',
        ['$scope', '$uibModalInstance', '$http', '$state', 'signalrMPTsService', 'entity',
        function ($scope, $uibModalInstance, $http, $state, signalrMPTsService, entity) {
            $scope.mpt = entity;

            $scope.$on('mptChanged', function (event, args) {
                if (args.UID === $scope.mpt.UID) {
                    $scope.mpt = args;
                    $scope.$apply();
                };
            });

            $scope.SetAutomaticState = function () {
                $http.post('MPTs/SetAutomaticState', { id: $scope.mpt.UID });
            };

            $scope.SetManualState = function () {
                $http.post('MPTs/SetManualState', { id: $scope.mpt.UID });
            };

            $scope.SetIgnoreState = function () {
                $http.post('MPTs/SetIgnoreState', { id: $scope.mpt.UID });
            };

            $scope.TurnOn = function () {
                $http.post('MPTs/TurnOn', { id: $scope.mpt.UID });
            };

            $scope.TurnOnNow = function () {
                $http.post('MPTs/TurnOnNow', { id: $scope.mpt.UID });
            };

            $scope.ForbidStart = function () {
                $http.post('MPTs/ForbidStart', { id: $scope.mpt.UID });
            };

            $scope.TurnOff = function () {
                $http.post('MPTs/TurnOff', { id: $scope.mpt.UID });
            };

            $scope.Show = function () {
                $state.go('MPTs', { uid: $scope.mpt.UID });
            };

            $scope.ShowJournal = function () {
                $state.go('archive', { uid: $scope.mpt.UID });
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