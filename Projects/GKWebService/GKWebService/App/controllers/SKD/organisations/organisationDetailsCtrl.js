(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('organisationDetailsCtrl',
        ['$scope', '$uibModal', '$uibModalInstance', 'organisations', 'organisation', 'isNew', 'organisationsService',
        function ($scope, $uibModal, $uibModalInstance, organisations, organisation, isNew, organisationsService) {
            $scope.isNew = isNew;

            $scope.organisations = organisations;

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
                        employees: organisationsService.getOrganisationEmployees($scope.organisation.UID),
                        selectedEmployeeUID: function () {
                            return ($scope.model.SelectedChief) ? $scope.model.SelectedChief.UID : null;
                        }
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
                        employees: organisationsService.getOrganisationEmployees($scope.organisation.UID),
                        selectedEmployeeUID: function () {
                            return ($scope.model.SelectedHRChief) ? $scope.model.SelectedHRChief.UID : null;
                        }
                    }
                });

                modalInstance.result.then(function (employee) {
                    $scope.model.SelectedHRChief = employee;
                });
            };

            $scope.save = function () {
                organisationsService.saveOrganisation($scope.model, $scope.isNew).then(function () {
                        $uibModalInstance.close($scope.organisation);
                    });
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
