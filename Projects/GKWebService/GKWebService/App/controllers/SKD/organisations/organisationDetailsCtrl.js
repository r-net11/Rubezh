(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('organisationDetailsCtrl',
        ['$scope', '$uibModal', '$uibModalInstance', 'organisation', 'isNew', 'organisationsService',
        function ($scope, $uibModal, $uibModalInstance, organisation, isNew, organisationsService) {
            $scope.isNew = isNew;

            $scope.organisation = organisation.Organisation;

            $scope.model = organisation;

            if ($scope.isNew) {
                $scope.title = "Создание новой организации";
            } else {
                $scope.title = "Свойства организации: " + $scope.organisation.Name;
            }

            $scope.selectChief = function() {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Hr/EmployeeSelectionDialog',
                    controller: 'employeeSelectionDialogCtrl',
                    backdrop: 'static',
                    resolve: {
                        employees: organisationsService.getOrganisationEmployees($scope.organisation.UID)
                    }
                });

                modalInstance.result.then(function (employee) {
                    $scope.model.SelectedChief = employee;
                });
            };

            $scope.selectHRChief = function() {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: 'Hr/EmployeeSelectionDialog',
                    controller: 'employeeSelectionDialogCtrl',
                    backdrop: 'static',
                    resolve: {
                        employees: organisationsService.getOrganisationEmployees($scope.organisation.UID)
                    }
                });

                modalInstance.result.then(function (employee) {
                    $scope.model.SelectedHRChief = employee;
                });
            };

            $scope.save = function () {
                organisationsService.saveOrganisation($scope.model, $scope.isNew).then(function () {
                        $uibModalInstance.close();
                    });
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
