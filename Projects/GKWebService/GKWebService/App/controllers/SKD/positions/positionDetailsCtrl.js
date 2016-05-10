(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('positionDetailsCtrl',
        ['$scope', '$uibModalInstance', 'position', 'positions', 'isNew', 'positionsService',
        function ($scope, $uibModalInstance, position, positions, isNew, positionsService) {
            $scope.isNew = isNew;

            $scope.position = position.Position;

            $scope.model = position;

            $scope.positions = positions;

            if ($scope.isNew) {
                $scope.title = "Создание должности";
            } else {
                $scope.title = "Свойства должности: " + $scope.position.Name;
            }

            $scope.save = function () {
                positionsService.savePosition($scope.model, $scope.isNew).then(function () {
                    $uibModalInstance.close($scope.position);
                });
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
