(function () {

    angular.module('gkApp.controllers').controller('mptsDetailsCtrl',
        ['$scope', '$uibModalInstance', '$http', 'signalrMPTsService', 'entity',
        function ($scope, $uibModalInstance, $http, signalrMPTsService, entity) {
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

            };

            $scope.ShowJournal = function () {

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