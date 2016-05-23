(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeeDetailsCtrl',
        ['$scope', '$uibModal', '$uibModalInstance', 'personType', 'employee', 'organisation', 'isNew', 'employeesService',
        function ($scope, $uibModal, $uibModalInstance, personType, employee, organisation, isNew, employeesService) {
            var emptyGuid = '00000000-0000-0000-0000-000000000000';

            $scope.model = {
                photoData: '',
                isOrganisationChief: false,
                isOrganisationHRChief: false
            };

            $scope.isNew = isNew;

            $scope.isEmployee = (personType === "Employee");

            $scope.employee = employee.Employee;

            $scope.employee.Type = personType;

            $scope.model.photoData = employee.PhotoData;

            $scope.popupDocumentValidTo = {
                opened: false
            };

            $scope.popupGivenDate = {
                opened: false
            };

            function FIO() {
                var names = [$scope.employee.LastName, $scope.employee.FirstName, $scope.employee.SecondName];
                return names.join(" ");
            };

            if ($scope.isNew) {
                $scope.title = ($scope.isEmployee ? "Добавить сотрудника" : "Добавить посетителя");
                $scope.employee.OrganisationUID = organisation.UID;
            } else {
                $scope.title = ($scope.isEmployee ? "Свойства сотрудника: " : "Свойства посетителя: ") + FIO();
                $scope.model.isOrganisationChief = (organisation.ChiefUID === $scope.employee.UID);
                $scope.model.isOrganisationHRChief = (organisation.HRChiefUID === $scope.employee.UID);
            }

            $scope.isPositionSelected = function () {
                return $scope.employee.PositionUID !== emptyGuid;
            };

            $scope.isDepartmentSelected = function () {
                return $scope.employee.DepartmentUID !== emptyGuid;
            };

            $scope.isEscortSelected = function () {
                return $scope.employee.EscortUID && $scope.employee.EscortUID !== emptyGuid;
            };

            $scope.isScheduleSelected = function () {
                return $scope.employee.ScheduleUID && $scope.employee.ScheduleUID !== emptyGuid;
            };

            $scope.save = function () {
                employeesService.saveEmployee($scope.employee,
                    $scope.model.photoData,
                    $scope.isNew,
                    organisation,
                    $scope.model.isOrganisationChief,
                    $scope.model.isOrganisationHRChief).then(function() {
                        $uibModalInstance.close($scope.employee);
                    });
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $scope.selectPosition = function() {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Employees/PositionSelection',
                    controller: 'positionSelectionCtrl',
                    backdrop: 'static',
                    resolve: {
                        organisationUID: function () {
                            return organisation.UID;
                        },
                        positionUID: function () {
                            return $scope.employee.PositionUID;
                        }
                    }
                });

                modalInstance.result.then(function (position) {
                    if (position) {
                        $scope.employee.PositionUID = position.UID;
                        $scope.employee.PositionName = position.Name;
                    } else {
                        $scope.employee.PositionUID = emptyGuid;
                        $scope.employee.PositionName = null;
                    }
                });
            };

            $scope.selectDepartment = function () {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Employees/DepartmentSelection',
                    controller: 'departmentSelectionCtrl',
                    backdrop: 'static',
                    resolve: {
                        organisationUID: function () {
                            return organisation.UID;
                        },
                        departmentUID: function () {
                            return null;
                        }
                    }
                });

                modalInstance.result.then(function (department) {
                    if (department) {
                        $scope.employee.DepartmentUID = department.UID;
                        $scope.employee.DepartmentName = department.Name;
                    } else {
                        $scope.employee.DepartmentUID = emptyGuid;
                        $scope.employee.DepartmentName = null;
                    }
                });
            };

            $scope.selectSchedule = function () {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Employees/ScheduleSelection',
                    controller: 'scheduleSelectionCtrl',
                    backdrop: 'static',
                    resolve: {
                        schedules: employeesService.getSchedules(organisation.UID),
                        scheduleStartDate: function() { return $scope.employee.ScheduleStartDate; }
                    }
                });

                modalInstance.result.then(function (result) {
                    if (result.selectedSchedule) {
                        $scope.employee.ScheduleUID = result.selectedSchedule.UID;
                        $scope.employee.ScheduleName = result.selectedSchedule.Name;
                    } else {
                        $scope.employee.ScheduleUID = emptyGuid;
                        $scope.employee.ScheduleName = null;
                    }
                    $scope.employee.ScheduleStartDate = result.scheduleStartDate;
                });
            };

            $scope.selectEscort = function () {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Hr/EmployeeSelectionDialog',
                    controller: 'employeeSelectionDialogCtrl',
                    backdrop: 'static',
                    resolve: {
                        employees: employeesService.getOrganisationDepartmentEmployees(organisation.UID, $scope.employee.DepartmentUID),
                        selectedEmployeeUID: function () {
                            return $scope.employee.EscortUID === emptyGuid ? null : $scope.employee.EscortUID;
                        }
                    }
                });

                modalInstance.result.then(function (employee) {
                    if (employee) {
                        $scope.employee.EscortUID = employee.UID;
                        $scope.employee.EscortName = employee.Name;
                    } else {
                        $scope.employee.EscortUID = null;
                        $scope.employee.EscortName = null;
                    }
                });
            };
        }]
    );

}());
