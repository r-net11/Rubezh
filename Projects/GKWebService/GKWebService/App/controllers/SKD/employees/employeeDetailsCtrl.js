(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeeDetailsCtrl',
        ['$scope', '$uibModalInstance', 'personType', 'employee', 'organisation',
        function ($scope, $uibModalInstance, personType, employee, organisation) {
            var emptyGuid = '00000000-0000-0000-0000-000000000000';

            $scope.isNew = !angular.isObject(employee);

            $scope.isEmployee = (personType === "Employee");

            $scope.employee = employee;

            if ($scope.isNew) {
                $scope.title = ($scope.isEmployee ? "Добавить сотрудника" : "Добавить посетителя");
                $scope.employee.OrganisationUID = organisation.UID;
                $scope.isOrganisationChief(false);
                $scope.isOrganisationHRChief(false);
            } else {
                $scope.title = ($scope.isEmployee() ? "Свойства сотрудника: " : "Свойства посетителя: ") + $scope.employee.FIO();
                $scope.isOrganisationChief(organisation.ChiefUID === $scope.employee.UID());
                $scope.isOrganisationHRChief(organisation.HRChiefUID === $scope.employee.UID());
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
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
