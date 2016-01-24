(function () {

    angular.module('canvasApp.controllers').controller('directionDetailsCtrl',
        function ($scope, $uibModalInstance, $http, direction) {
            $scope.direction = direction;

            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }
    );
}());
