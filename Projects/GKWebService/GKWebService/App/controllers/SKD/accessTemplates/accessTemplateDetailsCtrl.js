(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('accessTemplateDetailsCtrl',
        ['$scope', '$uibModalInstance', 'accessTemplate', 'isNew', 'accessTemplatesService',
        function ($scope, $uibModalInstance, accessTemplate, isNew, accessTemplatesService) {
            $scope.isNew = isNew;

            $scope.accessTemplate = accessTemplate.AccessTemplate;

            $scope.model = accessTemplate;

            $scope.accessTemplates = accessTemplatesService.accessTemplates;

            if ($scope.isNew) {
                $scope.title = "Создание шаблона доступа";
            } else {
                $scope.title = "Свойства шаблона доступа: " + $scope.accessTemplate.Name;
            }

            $scope.save = function () {
                accessTemplatesService.saveAccessTemplate($scope.model, $scope.isNew).then(function () {
                        $uibModalInstance.close();
                    });
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
