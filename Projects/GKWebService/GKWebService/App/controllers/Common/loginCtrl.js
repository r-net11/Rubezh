(function () {

    angular.module('gkApp.controllers').controller('loginCtrl',
        ['$scope', '$uibModalInstance', '$http', '$state', 'authService',
        function ($scope, $uibModalInstance, $http, $state, authService) {
            $scope.loginData = {
                userName: "adm",
                password: ""
            };

            $scope.message = "";

            $scope.Enter = function () {
                authService.login($scope.loginData).then(function (response) {
                    $scope.message = "";
                    $uibModalInstance.close();
                    $state.go('home');
                },
                function (err) {
                    $scope.message = err;
                });
            };
        }]
    );
}());