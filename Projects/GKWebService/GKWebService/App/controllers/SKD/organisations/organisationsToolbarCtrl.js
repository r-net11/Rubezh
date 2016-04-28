(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('organisationsToolbarCtrl',
        ['$scope', '$uibModal', 'organisationsService', 'dialogService', 'authService',
         function ($scope, $uibModal, organisationsService, dialogService, authService) {
             $scope.$watch(function() {
                 return organisationsService.selectedOrganisation;
             }, function (organisation) {
                 $scope.selectedOrganisation = organisation;
             });

             $scope.isOrganisationsEditAllowed = authService.checkPermission('Oper_SKD_Organisations_Edit');
             $scope.isOrganisationsAddRemoveAllowed = authService.checkPermission('Oper_SKD_Organisations_AddRemove');

             $scope.canAdd = function () {
                 return $scope.isOrganisationsAddRemoveAllowed;
             };
             $scope.canRemove = function () {
                 return $scope.selectedOrganisation && !$scope.selectedOrganisation.IsDeleted && $scope.isOrganisationsAddRemoveAllowed;
             };
             $scope.canEdit = function () {
                 return $scope.selectedOrganisation && !$scope.selectedOrganisation.IsDeleted && $scope.isOrganisationsEditAllowed;
             };
             $scope.canRestore = function () {
                 return $scope.selectedOrganisation && $scope.selectedOrganisation.IsDeleted && $scope.isOrganisationsAddRemoveAllowed;
             };

             var showOrganisationDetails = function (UID, isNew) {
                 var modalInstance = $uibModal.open({
                     animation: false,
                     templateUrl: 'Organisations/OrganisationDetails',
                     controller: 'organisationDetailsCtrl',
                     backdrop: 'static',
                     resolve: {
                         organisation: function () {
                             return organisationsService.getOrganisationDetails(UID);
                         },
                         organisations: function() {
                             return organisationsService.organisations;
                         },
                         isNew: function () {
                             return isNew;
                         }
                     }
                 });

                 modalInstance.result.then(function (organisation) {
                     if (isNew) {
                         organisationsService.reload(organisation.UID);
                         $scope.$parent.$broadcast('AddOrganisationEvent', organisation);
                     } else {
                         $scope.selectedOrganisation.Name = organisation.Name;
                         $scope.$parent.$broadcast('EditOrganisationEvent', organisation);
                     }
                 });
             };

             $scope.edit = function () {
                 showOrganisationDetails($scope.selectedOrganisation.UID, false);
             };

             $scope.add = function () {
                 showOrganisationDetails('', true);
             };

             $scope.remove = function () {
                 if (dialogService.showConfirm("Вы уверены, что хотите удалить огранизацию?")) {
                     organisationsService.isAnyOrganisationItems($scope.selectedOrganisation.UID).then(function (isAnyOrganisationItems) {
                         if (!isAnyOrganisationItems || 
                             dialogService.showConfirm("Привязанные к организации объекты будут также архивированы. Продолжить?")) {
                                 organisationsService.markDeleted($scope.selectedOrganisation.UID, $scope.selectedOrganisation.Name).then(function () {
                                     $scope.$parent.$broadcast('RemoveOrganisationEvent', $scope.selectedOrganisation);
                                });
                            }
                        }
                    );
                 }
             };

             $scope.changeIsDeleted = function () {
                 if ($scope.isWithDeleted())
                     $scope.filter.LogicalDeletationType = "Active";
                 else 
                     $scope.filter.LogicalDeletationType = "All";
             };
         }]
    );

}());
