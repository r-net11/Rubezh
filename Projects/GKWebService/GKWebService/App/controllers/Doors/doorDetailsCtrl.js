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
                $http.post('Doors/SetAutomaticState', { id: $scope.door.UID });
            }

            $scope.SetManualState = function() {
                $http.post('Doors/SetManualState', { id: $scope.door.UID });
            };

            $scope.SetIgnoreState = function() {
                $http.post('Doors/SetIgnoreState', { id: $scope.door.UID });
            };

            $scope.TurnOn = function() {
                $http.post('Doors/TurnOn', { id: $scope.door.UID });
            };

            $scope.TurnOffNow = function () {
                $http.post('Doors/TurnOffNow', { id: $scope.door.UID });
            };

            $scope.TurnOff = function() {
                $http.post('Doors/TurnOff', { id: $scope.door.UID });
            };

            $scope.Reset = function() {
                $http.post('Doors/Reset', { id: $scope.door.UID });
            };

            $scope.SetRegimeNorm = function() {
                $http.post('Doors/SetRegimeNorm', { id: $scope.door.UID });
            };

            $scope.SetRegimeOpen = function() {
                $http.post('Doors/SetRegimeOpen', { id: $scope.door.UID });
            };

            $scope.SetRegimeClose = function() {
                $http.post('Doors/SetRegimeClose', { id: $scope.door.UID });
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