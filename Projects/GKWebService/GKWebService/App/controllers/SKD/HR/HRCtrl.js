(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('HRCtrl', 
        ['$scope', 'authService',
        function ($scope, authService) {
            $scope.selectedPersonType = "Employee";

            $scope.canEmployeesView = function() {
                return authService.checkPermission('Oper_SKD_Employees_View');
            };

            $scope.canGuestsView = function () {
                return authService.checkPermission('Oper_SKD_Guests_View');
            };

            $scope.employeesHeader = function() {
                return ($scope.selectedPersonType === "Employee") ? "Сотрудники" : "Посетители";
            };

            $scope.canSelectEmployees = function() {
                return authService.checkPermission('Oper_SKD_Employees_View') || authService.checkPermission('Oper_SKD_Guests_View');
            };
            $scope.canSelectPositions = function () {
                return authService.checkPermission('Oper_SKD_Positions_View');
            };
            $scope.canSelectDepartments = function () {
                return authService.checkPermission('Oper_SKD_Departments_View');
            };
            $scope.canSelectCards = function () {
                return authService.checkPermission('Oper_SKD_Cards_View');
            };
            $scope.canSelectAccessTemplates = function () {
                return authService.checkPermission('Oper_SKD_AccessTemplates_View');
            };
            $scope.canSelectOrganisations = function () {
                return authService.checkPermission('Oper_SKD_Organisations_View');
            };
        }]
    );

}());
