(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('HRFilterCtrl', 
        ['$scope', '$uibModalInstance', 'filter',
        function ($scope, $uibModalInstance, filter) {
            $scope.filter = filter;

            $scope.save = function () {
                $scope.filter.LogicalDeletationType = ($scope.IsWithDeleted ? "All" : "Active");

                $uibModalInstance.close($scope.filter);
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );

}());
