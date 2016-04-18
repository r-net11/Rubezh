(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('cardsToolbarCtrl',
        ['$scope', '$uibModal', 'cardsService', 'dialogService', 'authService',
         function ($scope, $uibModal, cardsService, dialogService, authService) {
             $scope.$watch(function() {
                 return cardsService.selectedCard;
             }, function (card) {
                 $scope.selectedCard = card;
             });

             $scope.isCardsEditAllowed = authService.checkPermission('Oper_SKD_Cards_Etit');

             $scope.canRemove = function () {
                 return $scope.selectedCard && $scope.selectedCard.IsInStopList && !$scope.selectedCard.IsOrganisation && !$scope.selectedCard.IsDeactivatedRootItem && $scope.isCardsEditAllowed;
             };

             $scope.remove = function () {
                 if (dialogService.showConfirm("Вы уверены, что хотите удалить карту?")) {
                     cardsService.markDeleted($scope.selectedCard.UID).then(function () {
                        cardsService.reload();
                    });
                 }
             };
         }]
    );

}());
