﻿(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('positionsToolbarCtrl',
        ['$scope', '$uibModal', 'positionsService', 'dialogService', 'authService',
         function ($scope, $uibModal, positionsService, dialogService, authService) {
             $scope.$watch(function() {
                 return positionsService.selectedPosition;
             }, function (position) {
                 $scope.selectedPosition = position;
             });

             $scope.isPositionsEditAllowed = authService.checkPermission('Oper_SKD_Positions_Etit');

             $scope.canAdd = function () {
                 return $scope.selectedPosition && !$scope.selectedPosition.IsDeleted && $scope.isPositionsEditAllowed;
             };
             $scope.canRemove = function () {
                 return $scope.selectedPosition && !$scope.selectedPosition.IsDeleted && !$scope.selectedPosition.IsOrganisation && $scope.isPositionsEditAllowed;
             };
             $scope.canEdit = function () {
                 return $scope.selectedPosition && !$scope.selectedPosition.IsDeleted && !$scope.selectedPosition.IsOrganisation && $scope.isPositionsEditAllowed;
             };
             $scope.canCopy = function () {
                 return $scope.selectedPosition && !$scope.selectedPosition.IsDeleted && !$scope.selectedPosition.IsOrganisation && $scope.isPositionsEditAllowed;
             };
             $scope.canPaste = function () {
                 return $scope.selectedPosition && !$scope.selectedPosition.IsDeleted && $scope.Clipboard && $scope.isPositionsEditAllowed;
             };
             $scope.canRestore = function () {
                 return $scope.selectedPosition && $scope.selectedPosition.IsDeleted && !$scope.selectedPosition.IsOrganisation && $scope.isPositionsEditAllowed;
             };

             var showPositionDetails = function (UID, isNew) {
                 var modalInstance = $uibModal.open({
                     animation: false,
                     templateUrl: 'Positions/PositionDetails',
                     controller: 'positionDetailsCtrl',
                     backdrop: 'static',
                     resolve: {
                         position: function () {
                             return positionsService.getPositionDetails($scope.selectedPosition.OrganisationUID, UID);
                         },
                         isNew: function () {
                             return isNew;
                         }
                     }
                 });

                 modalInstance.result.then(function () {
                     positionsService.reload();
                 });
             };

             $scope.edit = function () {
                 showPositionDetails($scope.selectedPosition.UID, false);
             };

             $scope.add = function () {
                 showPositionDetails('', true);
             };

             $scope.remove = function () {
                 if (dialogService.showConfirm("Вы уверены, что хотите архивировать должность?")) {
                     positionsService.getChildEmployeeUIDs($scope.selectedPosition.UID).then(function (employeeUIDs) {
                         if (employeeUIDs.length > 0) {
                             if (dialogService.showConfirm("Существуют привязанные к должности сотрудники. Продолжить?")) {
                                positionsService.markDeleted($scope.selectedPosition.UID, $scope.selectedPosition.Name).then(function () {
                                    positionsService.reload();
                                });
                            }
                        } else {
                             positionsService.markDeleted($scope.selectedPosition.UID, $scope.selectedPosition.Name).then(function () {
                                positionsService.reload();
                            });
                        }
                    });
                 }
             };
         }]
    );

}());