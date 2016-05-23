(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('accessTemplatesToolbarCtrl',
        ['$scope', '$uibModal', 'accessTemplatesService', 'dialogService', 'authService',
         function ($scope, $uibModal, accessTemplatesService, dialogService, authService) {
             $scope.$watch(function() {
                 return accessTemplatesService.selectedAccessTemplate;
             }, function (accessTemplate) {
                 $scope.selectedAccessTemplate = accessTemplate;
             });

             $scope.isAccessTemplatesEditAllowed = authService.checkPermission('Oper_SKD_AccessTemplates_Etit');

             $scope.canAdd = function () {
                 return $scope.selectedAccessTemplate && !$scope.selectedAccessTemplate.IsDeleted && $scope.isAccessTemplatesEditAllowed;
             };
             $scope.canRemove = function () {
                 return $scope.selectedAccessTemplate && !$scope.selectedAccessTemplate.IsDeleted && !$scope.selectedAccessTemplate.IsOrganisation && $scope.isAccessTemplatesEditAllowed;
             };
             $scope.canEdit = function () {
                 return $scope.selectedAccessTemplate && !$scope.selectedAccessTemplate.IsDeleted && !$scope.selectedAccessTemplate.IsOrganisation && $scope.isAccessTemplatesEditAllowed;
             };
             $scope.canCopy = function () {
                 return $scope.selectedAccessTemplate && !$scope.selectedAccessTemplate.IsDeleted && !$scope.selectedAccessTemplate.IsOrganisation && $scope.isAccessTemplatesEditAllowed;
             };
             $scope.canPaste = function () {
                 return $scope.selectedAccessTemplate && !$scope.selectedAccessTemplate.IsDeleted && $scope.clipboard && $scope.isAccessTemplatesEditAllowed;
             };
             $scope.canRestore = function () {
                 return $scope.selectedAccessTemplate && $scope.selectedAccessTemplate.IsDeleted && !$scope.selectedAccessTemplate.IsOrganisation && $scope.isAccessTemplatesEditAllowed;
             };

             var showAccessTemplateDetails = function (UID, isNew) {
                 var modalInstance = $uibModal.open({
                     animation: false,
                     templateUrl: 'AccessTemplates/AccessTemplateDetails',
                     controller: 'accessTemplateDetailsCtrl',
                     backdrop: 'static',
                     resolve: {
                         accessTemplate: function () {
                             return accessTemplatesService.getAccessTemplateDetails($scope.selectedAccessTemplate.OrganisationUID, UID);
                         },
                         isNew: function () {
                             return isNew;
                         }
                     }
                 });

                 modalInstance.result.then(function (accessTemplate) {
                     if (isNew) {
                         $scope.$parent.$broadcast('AddAccessTemplateEvent', accessTemplate);
                     } else {
                         $scope.$parent.$broadcast('EditAccessTemplateEvent', accessTemplate);
                     }
                 });
             };

             $scope.edit = function () {
                 showAccessTemplateDetails($scope.selectedAccessTemplate.UID, false);
             };

             $scope.add = function () {
                 showAccessTemplateDetails('', true);
             };

             $scope.remove = function () {
                 if (dialogService.showConfirm("Вы уверены, что хотите архивировать шаблон доступа?")) {
                     accessTemplatesService.getLinkedCards($scope.selectedAccessTemplate.UID).then(function (numbersSting) {
                         if (numbersSting) {
                             if (dialogService.showConfirm("Шаблон привязан к пропускам номер " + numbersSting + ". При удалении шаблона указанные в нём точки доступа будут убраны из привязаных пропусков. Вы уверены, что хотите удалить шаблон?")) {
                                 accessTemplatesService.markDeleted($scope.selectedAccessTemplate.UID, $scope.selectedAccessTemplate.Name)
                                     .then(function () {
                                        accessTemplatesService.reload();
                                });
                            }
                        } else {
                             accessTemplatesService.markDeleted($scope.selectedAccessTemplate.UID, $scope.selectedAccessTemplate.Name)
                                 .then(function () {
                                    accessTemplatesService.reload();
                            });
                        }
                    });
                 }
             };

             $scope.copy = function () {
                 accessTemplatesService.getAccessTemplateDetails($scope.selectedAccessTemplate.OrganisationUID, $scope.selectedAccessTemplate.UID)
                     .then(function (accessTemplate) {
                        $scope.clipboard = accessTemplate;
                 });
             };

             $scope.paste = function () {
                 accessTemplatesService.paste($scope.selectedAccessTemplate.OrganisationUID, $scope.clipboard)
                     .then(function () {
                        accessTemplatesService.reload();
                 });
             };

             $scope.restore = function () {
                 $scope.restoreElement("шаблон доступа", accessTemplatesService.accessTemplates, $scope.selectedAccessTemplate).then(function () {
                     accessTemplatesService.restore($scope.selectedAccessTemplate).then(function () {
                         accessTemplatesService.reload();
                     });
                 });
             };
         }]
    );

}());
