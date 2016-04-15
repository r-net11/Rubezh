(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('positionEmployeeListCtrl',
        ['$scope', '$http', '$timeout', '$window', '$uibModal', 'positionsService', 'employeesService', 'authService',
        function ($scope, $http, $timeout, $window, $uibModal, positionsService, employeesService, authService) {
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
                    { field: 'FIO', width: 310, displayName: 'ФИО', cellTemplate: "<div class='ui-grid-cell-contents'><img style='vertical-align: middle; padding-right: 3px' ng-src='/Content/Image/Icon/Hr/Employee.png'/><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" },
                    { field: 'DepartmentName', width: 210, displayName: 'Подразделение' }
                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = ($window.innerHeight - 100) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            var reload = function () {
                if ($scope.selectedPosition && !$scope.selectedPosition.IsOrganisation) {
                    positionsService.getPositionEmployeeList($scope.filter).then(function (employees) {
                        $scope.gridOptions.data = employees;
                        $scope.selectedEmployee = null;
                    });
                } else {
                    $scope.gridOptions.data = null;
                    $scope.selectedEmployee = null;
                }
            };

            $scope.$watch(function () {
                return positionsService.selectedPosition;
            }, function (position) {
                $scope.selectedPosition = position;
                reload();
            });

            $scope.isShowEmployeeList = function () {
                return $scope.selectedPosition && !$scope.selectedPosition.IsOrganisation && authService.checkPermission('Oper_SKD_Employees_View');
            };

            $scope.isEmployeesEditAllowed = authService.checkPermission('Oper_SKD_Employees_Edit');

            $scope.canAdd = function () {
                return $scope.selectedPosition && !$scope.selectedPosition.IsDeleted && $scope.isEmployeesEditAllowed;
            };

            $scope.canRemove = function () {
                return $scope.selectedEmployee && !$scope.selectedEmployee.IsDeleted && !$scope.selectedPosition.IsDeleted && $scope.isEmployeesEditAllowed;
            };

            $scope.canEdit = function () {
                return $scope.selectedEmployee && !$scope.selectedEmployee.IsDeleted && !$scope.selectedPosition.IsDeleted && $scope.isEmployeesEditAllowed;
            };

            $scope.add = function() {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Hr/EmployeeSelectionDialog',
                    controller: 'employeeSelectionDialogCtrl',
                    backdrop: 'static',
                    resolve: {
                        employees: function () {
                            return $http.get('Hr/GetEmptyPositionEmployees/' + $scope.selectedPosition.OrganisationUID);
                        }
                    }
                });

                modalInstance.result.then(function (employee) {
                    return positionsService.saveEmployeePosition(employee, $scope.selectedPosition.UID);
                }).then(function() {
                    reload();
                });
            };

            $scope.remove = function () {
                positionsService.saveEmployeePosition($scope.selectedEmployee, null).then(function () {
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
        }]
    );

}());
