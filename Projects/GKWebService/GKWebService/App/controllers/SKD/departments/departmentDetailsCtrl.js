(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('departmentDetailsCtrl',
        ['$scope', '$uibModal', '$uibModalInstance', '$filter', 'department', 'departments', 'isNew', 'departmentsService',
        function ($scope, $uibModal, $uibModalInstance, $filter, department, departments, isNew, departmentsService) {
            $scope.isNew = isNew;

            $scope.department = department.Department;

            $scope.model = department;

            $scope.departments = $filter('filter')(departments, function (d) {
                return d.OrganisationUID === $scope.department.OrganisationUID && !d.IsOrganisation && d.ParentUID === $scope.department.ParentDepartmentUID;
            });

            if ($scope.isNew) {
                $scope.title = "Создание подразделения";
            } else {
                $scope.title = "Свойства подразделения: " + $scope.department.Name;
            }

            $scope.save = function () {
                departmentsService.saveDepartment($scope.model, $scope.isNew).then(function () {
                    $scope.department.ParentDepartmentUID = $scope.model.SelectedDepartment ? $scope.model.SelectedDepartment.UID : "";
                    $uibModalInstance.close($scope.department);
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
                        $scope.model.SelectedDepartment = { UID: department.UID, Name: department.Name };
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
                        employees: departmentsService.getDepartmentEmployees($scope.department.UID),
                        selectedEmployeeUID: function () {
                            return ($scope.model.SelectedChief) ? $scope.model.SelectedChief.UID : null;
                        }
            }
                });

                modalInstance.result.then(function (employee) {
                    $scope.model.SelectedChief = employee;
                });
            };
        }]
    );

}());
