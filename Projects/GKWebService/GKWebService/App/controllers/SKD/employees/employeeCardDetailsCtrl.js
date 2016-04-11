(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeeCardDetailsCtrl',
        ['$scope', '$uibModalInstance', 'personType', 'card', 'organisationUID', 'employee', 'isNew', 'employeesService', 'authService', 'dialogService',
        function ($scope, $uibModalInstance, personType, card, organisationUID, employee, isNew, employeesService, authService, dialogService) {
            $scope.isNew = isNew;

            $scope.card = card;

            $scope.isGuest = (personType === "Guest");

            $scope.canSelectGKControllers = function () {
                return $scope.card.Card.GKCardType !== 0;
            };

            $scope.canChangeCardType = !$scope.isGuest && authService.checkPermission('Oper_SKD_Employees_Edit_CardType');

            if ($scope.isNew) {
                $scope.title = "Создание пропуска";
            } else {
                $scope.title = "Свойства пропуска: " + $scope.card.Card.Number;
            }

            $scope.popupEndDate = {
                opened: false
            };

            $scope.$watch('card.SelectedStopListCard', function (card) {
                if (card) {
                    $scope.card.Card.Number = card.Number;
                }
            });

            $scope.$watch('card.UseStopList', function (useStopList) {
                if (useStopList && card.SelectedStopListCard) {
                    $scope.card.Card.Number = card.SelectedStopListCard.Number;
                }
            });

            $scope.save = function () {
                var stopListCard = null;
                for (var i = 0; i < card.StopListCards.length; i++) {
                    if (card.Card.Number === card.StopListCards[i].Number) {
                        stopListCard = card.StopListCards[i];
                        break;
                    }
                }

                if (stopListCard) {
                    if (dialogService.showConfirm("Карта с таким номером находится в стоп-листе. Использовать её?")) {
                        card.UseStopList = true;
                        card.SelectedStopListCard = stopListCard;
                        employeesService.saveEmployeeCardDetails(card, employee, organisationUID, isNew).then(function () {
                            $uibModalInstance.close();
                        });
                    } else {
                        return;
                    }
                } else {
                    employeesService.saveEmployeeCardDetails(card, employee, organisationUID, isNew).then(function () {
                        $uibModalInstance.close();
                    });
                }
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
