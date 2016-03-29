(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('HRCtrl', 
        ['$scope', '$uibModal', 'authData',
        function ($scope, $uibModal, authData) {
            $scope.canEmployeesView = function() {
                return authData.checkPermission('Oper_SKD_Employees_View');
            };

            $scope.canGuestsView = function () {
                return authData.checkPermission('Oper_SKD_Guests_View');
            };

            $scope.employeesHeader = function() {
                return ($scope.selectedPersonType === "Employee") ? "Сотрудники" : "Посетители";
            };

            $scope.canSelectEmployees = function() {
                return authData.checkPermission('Oper_SKD_Employees_View') || authData.checkPermission('Oper_SKD_Guests_View');
            };
            $scope.canSelectPositions = function () {
                return authData.checkPermission('Oper_SKD_Positions_View');
            };
            $scope.canSelectDepartments = function () {
                return authData.checkPermission('Oper_SKD_Departments_View');
            };
            $scope.canSelectCards = function () {
                return authData.checkPermission('Oper_SKD_Cards_View');
            };
            $scope.canSelectAccessTemplates = function () {
                return authData.checkPermission('Oper_SKD_AccessTemplates_View');
            };
            $scope.canSelectOrganisations = function () {
                return authData.checkPermission('Oper_SKD_Organisations_View');
            };

            $scope.$watch('selectedPersonType', function (newValue, oldValue) {
                $scope.filter.UIDs = [];
                $scope.filter.PositionUIDs = [];
                $scope.initializeEmployeeFilter();
            });

            $scope.initializeEmployeeFilter = function () {
                $scope.filter.PersonType = $scope.selectedPersonType;
            };

            if ($scope.canEmployeesView()) {
                $scope.selectedPersonType = "Employee";
            } else if ($scope.canGuestsView()) {
                $scope.selectedPersonType = "Guest";
            }

            $scope.filter = {};

            $scope.editFilter = function () {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Hr/HrFilter',
                    controller: 'HRFilterCtrl',
                    backdrop: 'static',
                    resolve: {
                        filter: function () {
                            return $scope.filter;
                        }
                    }
                });
            };
        }]
    );

}());
