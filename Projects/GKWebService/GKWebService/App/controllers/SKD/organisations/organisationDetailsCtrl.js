(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('organisationDetailsCtrl',
        ['$scope', '$uibModalInstance', 'organisation', 'isNew', 'organisationsService',
        function ($scope, $uibModalInstance, organisation, isNew, organisationsService) {
            $scope.isNew = isNew;

            $scope.organisation = organisation.Organisation;

            $scope.model = organisation;

            if ($scope.isNew) {
                $scope.title = "Создание новой организации";
            } else {
                $scope.title = "Свойства организации: " + $scope.organisation.Name;
            }

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
