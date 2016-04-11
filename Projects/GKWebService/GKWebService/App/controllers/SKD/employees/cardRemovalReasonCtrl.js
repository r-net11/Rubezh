(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('cardRemovalReasonCtrl',
        ['$scope', '$uibModalInstance', 'dateFilter',
        function ($scope, $uibModalInstance, dateFilter) {
            $scope.model = {
                removeType: 'remove',
                removalReason: "Утерян " + dateFilter(new Date(), 'dd.MM.yyyy'),
            };

            $scope.canSave = function () {
                return $scope.model.removeType !== 'deactivate' || $scope.model.removalReason;
            };

            $scope.okClick = function() {
                $uibModalInstance.close($scope.model);
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
