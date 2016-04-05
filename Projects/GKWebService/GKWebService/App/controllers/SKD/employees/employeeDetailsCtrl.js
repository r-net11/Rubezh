(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeeDetailsCtrl',
        ['$scope', '$uibModalInstance', 'personType', 'employee', 'organisation', 'employeesService',
        function ($scope, $uibModalInstance, personType, employee, organisation, employeesService) {
            var emptyGuid = '00000000-0000-0000-0000-000000000000';

            $scope.model = {
                photoData: '',
                isOrganisationChief: false,
                isOrganisationHRChief: false
            };

            $scope.isNew = !angular.isObject(employee);

            $scope.isEmployee = (personType === "Employee");

            $scope.employee = employee.Employee;

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
                        $uibModalInstance.close();
                    });
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
