(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('departmentDetailsCtrl',
        ['$scope', '$uibModalInstance', 'department', 'isNew', 'departmentsService',
        function ($scope, $uibModalInstance, department, isNew, departmentsService) {
            $scope.isNew = isNew;

            $scope.department = department.Department;

            $scope.model = department;

            if ($scope.isNew) {
                $scope.title = "Создание подразделения";
            } else {
                $scope.title = "Свойства подразделения: " + $scope.department.Name;
            }

            $scope.save = function () {
                departmentsService.saveDepartment($scope.model, $scope.isNew).then(function() {
                        $uibModalInstance.close();
                    });
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
