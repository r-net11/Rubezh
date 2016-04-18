(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeeSelectionDialogCtrl',
        ['$scope', '$uibModalInstance', 'employees',
        function ($scope, $uibModalInstance, employees) {
            $scope.employees = employees.data.Employees;

            $scope.model = {
                selectedEmployee: null
            };

            $scope.save = function() {
                $uibModalInstance.close($scope.model.selectedEmployee);
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
