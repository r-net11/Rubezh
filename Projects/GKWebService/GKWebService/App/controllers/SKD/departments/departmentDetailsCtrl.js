(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('departmentDetailsCtrl',
        ['$scope', '$uibModal', '$uibModalInstance', 'department', 'isNew', 'departmentsService',
        function ($scope, $uibModal, $uibModalInstance, department, isNew, departmentsService) {
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

            $scope.selectDepartment = function () {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Employees/DepartmentSelection',
                    controller: 'departmentSelectionCtrl',
                    backdrop: 'static',
                    resolve: {
                        organisationUID: function () {
                            return $scope.department.OrganisationUID;
                        },
                        departmentUID: function () {
                            return $scope.department.UID;
                        }
                    }
                });

                modalInstance.result.then(function (department) {
                    if (department) {
                        $scope.model.SelectedDepartment.UID = department.UID;
                        $scope.model.SelectedDepartment.Name = department.Name;
                    } else {
                        $scope.model.SelectedDepartment = null;
                    }
                });
            };

            $scope.selectChief = function () {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Hr/EmployeeSelectionDialog',
                    controller: 'employeeSelectionDialogCtrl',
                    backdrop: 'static',
                    resolve: {
                        employees: departmentsService.getDepartmentEmployees($scope.department.UID)
                    }
                });

                modalInstance.result.then(function (employee) {
                    $scope.model.SelectedChief = employee;
                });
            };
        }]
    );

}());
