(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('departmentEmployeeListCtrl',
        ['$scope', '$http', '$timeout', '$window', '$uibModal', 'departmentsService', 'employeesService', 'authService',
        function ($scope, $http, $timeout, $window, $uibModal, departmentsService, employeesService, authService) {
            $scope.gridOptions = {
                onRegisterApi: function(gridApi) {
                    $scope.gridApi = gridApi;
                    gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                        $scope.selectedEmployee = row.entity;
                    });
                },
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableRowSelection: true,
                noUnselect: true,
                columnDefs: [
                    { field: 'FIO', width: 210, displayName: 'ФИО', cellTemplate: "<div class='ui-grid-cell-contents'><img style='vertical-align: middle; padding-right: 3px' ng-src='/Content/Image/Icon/Hr/Employee.png'/><img ng-if='row.entity.IsChief' style='vertical-align: middle; padding-right: 3px' ng-src='/Content/Image/Icon/Hr/Chief.png' /><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" },
                    { field: 'PositionName', width: 210, displayName: 'Должность' }
                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = ($window.innerHeight - 100) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            var reload = function () {
                if ($scope.selectedDepartment && !$scope.selectedDepartment.IsOrganisation) {
                    departmentsService.detDepartmentEmployeeList($scope.filter).then(function(employees) {
                        $scope.gridOptions.data = employees;
                        $scope.selectedEmployee = departmentsService.selectedEmployee = null;
                    });
                } else {
                    $scope.gridOptions.data = null;
                    $scope.selectedEmployee = departmentsService.selectedEmployee = null;
                }
            };

            $scope.$watch(function () {
                return departmentsService.selectedDepartment;
            }, function (department) {
                $scope.selectedDepartment = department;
                reload();
            });

            $scope.isShowEmployeeList = function () {
                return $scope.selectedDepartment && !$scope.selectedDepartment.IsOrganisation && authService.checkPermission('Oper_SKD_Employees_View');
            };

            $scope.isEmployeesEditAllowed = authService.checkPermission('Oper_SKD_Employees_Edit');

            $scope.canAdd = function () {
                return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && $scope.isEmployeesEditAllowed;
            };

            $scope.canRemove = function () {
                return $scope.selectedEmployee && !$scope.selectedEmployee.IsDeleted && !$scope.selectedDepartment.IsDeleted && $scope.isEmployeesEditAllowed;
            };

            $scope.canEdit = function () {
                return $scope.selectedEmployee && !$scope.selectedEmployee.IsDeleted && !$scope.selectedDepartment.IsDeleted && $scope.isEmployeesEditAllowed;
            };

            $scope.canSetChief = function () {
                return $scope.selectedEmployee && !$scope.selectedEmployee.IsDeleted && !$scope.selectedEmployee.IsChief && !$scope.selectedDepartment.IsDeleted && $scope.isEmployeesEditAllowed;
            };

            $scope.canUnSetChief = function () {
                return $scope.selectedEmployee && !$scope.selectedEmployee.IsDeleted && $scope.selectedEmployee.IsChief && !$scope.selectedDepartment.IsDeleted && $scope.isEmployeesEditAllowed;
            };

            $scope.add = function() {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Hr/EmployeeSelectionDialog',
                    controller: 'employeeSelectionDialogCtrl',
                    backdrop: 'static',
                    resolve: {
                        employees: function () {
                            return $http.get('Hr/GetEmptyDepartmentEmployees/' + $scope.selectedDepartment.OrganisationUID);
                        }
                    }
                });

                modalInstance.result.then(function (employee) {
                    return departmentsService.saveEmployeeDepartment(employee, $scope.selectedDepartment.UID);
                }).then(function() {
                    reload();
                });
            };

            $scope.remove = function () {
                departmentsService.saveEmployeeDepartment($scope.selectedEmployee, null).then(function () {
                    reload();
                });
            };

            $scope.edit = function () {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Employees/EmployeeDetails',
                    controller: 'employeeDetailsCtrl',
                    backdrop: 'static',
                    resolve: {
                        employee: function () {
                            return employeesService.getEmployeeDetails($scope.selectedEmployee.UID);
                        },
                        personType: function () {
                            return $scope.filter.PersonType;
                        },
                        isNew: function () {
                            return false;
                        },
                        organisation: function () {
                            return employeesService.getOrganisation($scope.selectedEmployee.OrganisationUID);
                        }
                    }
                });

                modalInstance.result.then(function () {
                    reload();
                });
            };

            $scope.setChief = function () {
                departmentsService.saveDepartmentChief($scope.selectedDepartment, $scope.selectedEmployee.UID).then(function () {
                    $scope.selectedDepartment.Model.ChiefUID = $scope.selectedEmployee.UID;
                    reload();
                });
            };

            $scope.unSetChief = function () {
                departmentsService.saveDepartmentChief($scope.selectedDepartment, null).then(function () {
                    $scope.selectedDepartment.Model.ChiefUID = "00000000-0000-0000-0000-000000000000";
                    reload();
                });
            };
        }]
    );

}());
