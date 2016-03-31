(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeesCtrl',
        ['$scope', '$uibModal', 'employeesService',
         function ($scope, $uibModal, employeesService) {
             $scope.$watch(function() {
                 return employeesService.selectedEmployee;
             }, function(employee) {
                 $scope.selectedEmployee = employee;
             });

             $scope.editEmployeeClick = function() {
                 $uibModal.open({
                     templateUrl: 'Employees/EmployeeDetails',
                     controller: 'employeeDetailsCtrl',
                     backdrop: 'static',
                     resolve: {
                         employee: function () {
                             return employeesService.getEmployeeDetails($scope.selectedEmployee.UID);
                         }
                     }
                 });
             };
         }]
    );

}());
