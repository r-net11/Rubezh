(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeeSelectionDialogCtrl',
        ['$scope', '$uibModalInstance', 'selectedEmployeeUID', 'employees',
        function ($scope, $uibModalInstance, selectedEmployeeUID, employees) {
            $scope.employees = employees.data.Employees;

            $scope.model = {
                selectedEmployee: null
            };

            if (selectedEmployeeUID) {
                for (var i = 0; i < $scope.employees.length; i++) {
                    if ($scope.employees[i].UID === selectedEmployeeUID) {
                        $scope.model.selectedEmployee = $scope.employees[i];
                    } 
                }
            }

            $scope.save = function() {
                $uibModalInstance.close($scope.model.selectedEmployee);
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
