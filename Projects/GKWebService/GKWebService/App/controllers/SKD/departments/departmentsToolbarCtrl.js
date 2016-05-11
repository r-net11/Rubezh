(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('departmentsToolbarCtrl',
        ['$scope', '$uibModal', 'departmentsService', 'dialogService', 'authService',
         function ($scope, $uibModal, departmentsService, dialogService, authService) {
             $scope.$watch(function() {
                 return departmentsService.selectedDepartment;
             }, function (department) {
                 $scope.selectedDepartment = department;
             });

             $scope.isDepartmentsEditAllowed = authService.checkPermission('Oper_SKD_Departments_Etit');

             $scope.canAdd = function () {
                 return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && $scope.isDepartmentsEditAllowed;
             };
             $scope.canRemove = function () {
                 return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && !$scope.selectedDepartment.IsOrganisation && $scope.isDepartmentsEditAllowed;
             };
             $scope.canEdit = function () {
                 return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && !$scope.selectedDepartment.IsOrganisation && $scope.isDepartmentsEditAllowed;
             };
             $scope.canCopy = function () {
                 return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && !$scope.selectedDepartment.IsOrganisation && $scope.isDepartmentsEditAllowed;
             };
             $scope.canPaste = function () {
                 return $scope.selectedDepartment && !$scope.selectedDepartment.IsDeleted && $scope.Clipboard && $scope.isDepartmentsEditAllowed;
             };
             $scope.canRestore = function () {
                 return $scope.selectedDepartment && $scope.selectedDepartment.IsDeleted && !$scope.selectedDepartment.IsOrganisation && $scope.isDepartmentsEditAllowed;
             };

             var showDepartmentDetails = function(UID, isNew, parentUID) {
                 var modalInstance = $uibModal.open({
                     animation: false,
                     templateUrl: 'Departments/DepartmentDetails',
                     controller: 'departmentDetailsCtrl',
                     backdrop: 'static',
                     resolve: {
                         department: function () {
                             return departmentsService.getDepartmentDetails($scope.selectedDepartment.OrganisationUID, UID, parentUID);
                         },
                         isNew: function () {
                             return isNew;
                         },
                         departments: function() {
                             return departmentsService.departments;
                         }
                     }
                 });

                 modalInstance.result.then(function () {
                     departmentsService.reload();
                 });
             };

             $scope.edit = function () {
                 showDepartmentDetails($scope.selectedDepartment.UID, false, $scope.selectedDepartment.ParentUID);
             };

             $scope.add = function () {
                 showDepartmentDetails('', true, $scope.selectedDepartment.UID);
             };

             $scope.remove = function () {
                 if (dialogService.showConfirm("Вы уверены, что хотите архивировать подразделение?")) {
                     departmentsService.getChildEmployeeUIDs($scope.selectedDepartment.UID).then(function (departmentUIDs) {
                        if (departmentUIDs.length > 0) {
                            if (dialogService.showConfirm("Существуют привязанные к подразделению сотрудники. Продолжить?")) {
                                departmentsService.markDeleted($scope.selectedDepartment.UID).then(function() {
                                    departmentsService.reload();
                                });
                            }
                        } else {
                            departmentsService.markDeleted($scope.selectedDepartment.UID).then(function () {
                                departmentsService.reload();
                            });
                        }
                    });
                 }
             };

             $scope.restore = function () {
                 $scope.restoreElement("подразделение", departmentsService.departments, $scope.selectedDepartment).then(function (){
                     departmentsService.restore($scope.selectedDepartment).then(function () {
                         departmentsService.reload();
                     });
                 });
             };
         }]
    );

}());
