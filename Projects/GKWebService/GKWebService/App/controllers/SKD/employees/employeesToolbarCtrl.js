(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeesToolbarCtrl',
        ['$scope', '$uibModal', 'employeesService', 'dialogService', 
         function ($scope, $uibModal, employeesService, dialogService) {
             $scope.$watch(function() {
                 return employeesService.selectedEmployee;
             }, function(employee) {
                 $scope.selectedEmployee = employee;
             });

             $scope.$watch(function() {
                 return employeesService.selectedCard;
             }, function(card) {
                 $scope.selectedCard = card;
             });

             var isGuest = function() {
                 return $scope.selectedPersonType === "Guest";
             };
             var itemRemovingName = function () {
                 return isGuest() ? "посетителя" : "сотрудника";
             };
             $scope.addCommandToolTip = function () {
                 return "Добавить " + itemRemovingName();
             };
             $scope.removeCommandToolTip = function () {
                 return "Удалить " + itemRemovingName();
             };
             $scope.editCommandToolTip = function () {
                 return "Редактировать " + itemRemovingName();
             };
             var isEditAllowed = function () {
                 return isGuest() ? $scope.authData.checkPermission('Oper_SKD_Guests_Edit') : $scope.authData.checkPermission('Oper_SKD_Employees_Edit');
             };
             $scope.canAdd = function () {
                 return $scope.selectedEmployee && !$scope.selectedEmployee.IsDeleted && isEditAllowed();
             };
             $scope.canRemove = function () {
                 return $scope.selectedEmployee && !$scope.selectedEmployee.IsDeleted && !$scope.selectedEmployee.IsOrganisation && isEditAllowed();
             };
             $scope.canEdit = function () {
                 return $scope.selectedEmployee && !$scope.selectedEmployee.IsDeleted && !$scope.selectedEmployee.IsOrganisation && isEditAllowed();
             };
             $scope.canRestore = function () {
                 return $scope.selectedEmployee && $scope.selectedEmployee.IsDeleted && !$scope.selectedEmployee.IsOrganisation && isEditAllowed();
             };

             var showEmployeeDetails = function(UID, isNew) {
                 var modalInstance = $uibModal.open({
                     animation: false,
                     templateUrl: 'Employees/EmployeeDetails',
                     controller: 'employeeDetailsCtrl',
                     backdrop: 'static',
                     resolve: {
                         employee: function () {
                             return employeesService.getEmployeeDetails(UID);
                         },
                         personType: function () {
                             return $scope.filter.PersonType;
                         },
                         isNew: function () {
                             return isNew;
                         },
                         organisation: function () {
                             return employeesService.getOrganisation($scope.selectedEmployee.OrganisationUID);
                         }
                     }
                 });

                 modalInstance.result.then(function () {
                     employeesService.reload();
                 });
             };

             $scope.editEmployeeClick = function () {
                 showEmployeeDetails($scope.selectedEmployee.UID, false);
             };

             $scope.addEmployeeClick = function () {
                 showEmployeeDetails('', true);
             };

             var showEmployeeCardDetails = function (UID, isNew) {
                 var modalInstance = $uibModal.open({
                     animation: false,
                     templateUrl: 'Employees/EmployeeCardDetails',
                     controller: 'employeeCardDetailsCtrl',
                     backdrop: 'static',
                     resolve: {
                         card: function () {
                             return employeesService.getEmployeeCardDetails(UID, $scope.selectedEmployee.OrganisationUID);
                         },
                         personType: function () {
                             return $scope.filter.PersonType;
                         },
                         isNew: function () {
                             return isNew;
                         },
                         organisationUID: function () {
                             return $scope.selectedEmployee.OrganisationUID;
                         },
                         employee: function() {
                             return $scope.selectedEmployee;
                         }
                     }
                 });

                 modalInstance.result.then(function () {
                     employeesService.reload();
                 });
             };

             $scope.editEmployeeCardClick = function () {
                 showEmployeeCardDetails($scope.selectedCard.UID, false);
             };

             $scope.addEmployeeCardClick = function () {
                 showEmployeeCardDetails('', true);
             };

             $scope.removeEmployeeClick = function () {
                 if (dialogService.showConfirm("Вы уверены, что хотите архивировать " + itemRemovingName() + "?")) {
                     employeesService.markDeleted($scope.selectedEmployee).then(function() {
                         employeesService.reload();
                     });
                 }
             };

             $scope.removeEmployeeCardClick = function () {
                 if (dialogService.showConfirm("Вы уверены, что хотите архивировать " + itemRemovingName() + "?")) {
                     employeesService.markDeleted($scope.selectedEmployee).then(function() {
                         employeesService.reload();
                     });
                 }
             };

             $scope.canAddCard = function () {
                 return $scope.authData.checkPermission('Oper_SKD_Cards_Etit') && $scope.selectedEmployee && !$scope.selectedEmployee.IsOrganisation && !$scope.selectedEmployee.IsDeleted;
             };

             $scope.canEditCard = function () {
                 return $scope.authData.checkPermission('Oper_SKD_Cards_Etit');
             };
         }]
    );

}());
