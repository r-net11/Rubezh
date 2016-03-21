(function () {

    angular.module('gkApp.controllers').controller('mptsDetailsCtrl',
        ['$scope', '$uibModalInstance', '$http', '$state', 'signalrMPTsService', 'entity', 'authService',
        function ($scope, $uibModalInstance, $http, $state, signalrMPTsService, entity, authService) {
            $scope.mpt = entity;

            $scope.$on('mptChanged', function (event, args) {
                if (args.UID === $scope.mpt.UID) {
                    $scope.mpt = args;
                    $scope.$apply();
                };
            });

            $scope.SetAutomaticState = function () {
                authService.сonfirm().then(function (options) {
                    $http.post('MPTs/SetAutomaticState', { id: $scope.mpt.UID }, options);
                });
            };

            $scope.SetManualState = function () {
                authService.сonfirm().then(function (options) {
                    $http.post('MPTs/SetManualState', { id: $scope.mpt.UID }, options);
                });
            };

            $scope.SetIgnoreState = function () {
                authService.сonfirm().then(function (options) {
                    $http.post('MPTs/SetIgnoreState', { id: $scope.mpt.UID }, options);
                });
            };

            $scope.TurnOn = function () {
                authService.сonfirm().then(function (options) {
                    $http.post('MPTs/TurnOn', { id: $scope.mpt.UID }, options);
                });
            };

            $scope.TurnOnNow = function () {
                authService.сonfirm().then(function (options) {
                    $http.post('MPTs/TurnOnNow', { id: $scope.mpt.UID }, options);
                });
            };

            $scope.ForbidStart = function () {
                authService.сonfirm().then(function (options) {
                    $http.post('MPTs/ForbidStart', { id: $scope.mpt.UID }, options);
                });
            };

            $scope.TurnOff = function () {
                authService.сonfirm().then(function (options) {
                    $http.post('MPTs/TurnOff', { id: $scope.mpt.UID }, options);
                });
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