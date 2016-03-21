(function () {
    angular.module('gkApp').controller('doorDetailsCtrl',
    ['$scope', '$uibModalInstance', '$http', '$state', 'signalrDoorsService', 'entity', 'authService',
        function ($scope, $uibModalInstance, $http, $state, signalrDoorsService, entity, authService) {

            $scope.door = entity;
            $scope.FullCanControl = authService.checkPermission('Oper_Full_Door_Control');
            $scope.CanControl = authService.checkPermission('Oper_Door_Control');
            $scope.fulControl = $scope.FullCanControl;
            $scope.canControl = !$scope.fulControl;
            $scope.$on('doorChanged', function(event, args) {
                if (args.UID === $scope.door.UID) {
                    $scope.door = args;
                    $scope.$apply();
                };
            });

            

            $scope.SetAutomaticState = function() {
                authService.сonfirm().then(function(options) {
                    $http.post('Doors/SetAutomaticState', { id: $scope.door.UID }, options);
                });
            }

            $scope.SetManualState = function() {
                authService.сonfirm().then(function(options) {
                    $http.post('Doors/SetManualState', { id: $scope.door.UID }, options);
                });
            };

            $scope.SetIgnoreState = function() {
                authService.сonfirm().then(function(options) {
                    $http.post('Doors/SetIgnoreState', { id: $scope.door.UID }, options);
                });
            };

            $scope.TurnOn = function() {
                authService.сonfirm().then(function(options) {
                    $http.post('Doors/TurnOn', { id: $scope.door.UID }, options);
                });
            };

            $scope.TurnOffNow = function () {
                authService.сonfirm().then(function(options) {
                    $http.post('Doors/TurnOffNow', { id: $scope.door.UID }, options);
                });
            };

            $scope.TurnOff = function() {
                authService.сonfirm().then(function(options) {
                    $http.post('Doors/TurnOff', { id: $scope.door.UID }, options);
                });
            };

            $scope.Reset = function() {
                authService.сonfirm().then(function(options) {
                    $http.post('Doors/Reset', { id: $scope.door.UID }, options);
                });
            };

            $scope.SetRegimeNorm = function() {
                authService.сonfirm().then(function(options) {
                    $http.post('Doors/SetRegimeNorm', { id: $scope.door.UID }, options);
                });
            };

            $scope.SetRegimeOpen = function() {
                authService.сonfirm().then(function(options) {
                    $http.post('Doors/SetRegimeOpen', { id: $scope.door.UID }, options);
                });
            };

            $scope.SetRegimeClose = function() {
                authService.сonfirm().then(function(options) {
                    $http.post('Doors/SetRegimeClose', { id: $scope.door.UID }, options);
                });
            };

            $scope.Show = function() {
                $state.go('doors', { uid: $scope.door.UID });
            };

            $scope.ShowJournal = function() {
                $state.go('archive', { uid: $scope.door.UID });
            };

            $scope.ok = function() {
                $uibModalInstance.close();
            };

            $scope.cancel = function() {
                $uibModalInstance.dismiss('cancel');
            };
        }
    ]);
}());