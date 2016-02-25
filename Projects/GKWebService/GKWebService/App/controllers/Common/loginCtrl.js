(function () {

    angular.module('gkApp.controllers').controller('loginCtrl',
        ['$scope', '$uibModalInstance', '$http', '$state',
        function ($scope, $uibModalInstance, $http, $state) {
            $scope.Enter = function () {
                $uibModalInstance.close();
            };
        }]
    );
}());