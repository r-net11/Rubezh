(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('positionDetailsCtrl',
        ['$scope', '$uibModalInstance', 'position', 'isNew', 'positionsService',
        function ($scope, $uibModalInstance, position, isNew, positionsService) {
            $scope.isNew = isNew;

            $scope.position = position.Position;

            $scope.model = position;

            if ($scope.isNew) {
                $scope.title = "Создание должности";
            } else {
                $scope.title = "Свойства должности: " + $scope.position.Name;
            }

            $scope.save = function () {
                positionsService.savePosition($scope.model, $scope.isNew).then(function () {
                        $uibModalInstance.close();
                    });
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
