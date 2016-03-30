(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('HRFilterCtrl', 
        ['$scope', '$uibModalInstance', 'filter', 'personType',
        function ($scope, $uibModalInstance, filter, personType) {
            $scope.isShowPositions = (personType === "Employee");

            $scope.employeesHeader = (personType === "Employee") ? "Сотрудники" : "Посетители";

            $scope.personType = personType;

            $scope.filter = angular.copy(filter);

            if (filter.LogicalDeletationType) {
                $scope.isWithDeleted = (filter.LogicalDeletationType === "All");
            }

            $scope.save = function () {
                $scope.filter.LogicalDeletationType = ($scope.isWithDeleted ? "All" : "Active");
                $uibModalInstance.close($scope.filter);
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
