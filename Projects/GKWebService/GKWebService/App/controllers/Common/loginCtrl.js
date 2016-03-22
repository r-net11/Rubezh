(function () {

    angular.module('gkApp.controllers').controller('loginCtrl',
        ['$scope', '$uibModalInstance', '$http', '$state', 'authService', 'options',
        function ($scope, $uibModalInstance, $http, $state, authService, options) {
            $scope.loginData = {
                userName: "adm",
                password: ""
            };

            $scope.message = "";

            $scope.options = options;

            if ($scope.options.validateOnlyPassword) {
                $scope.loginData.userName = authService.authentication.userName;
            }

            $scope.Enter = function () {
                if (options.validateOnlyPassword) {
                    $http.post("Home/CheckPass", { password: $scope.loginData.password }).then(function (response) {
                        if (response.data.result) {
                            $uibModalInstance.close($scope.loginData.password);
                        } else {
                            // TODO: Нужно реализовать общее окно для отображения ошибок
                            alert("Неверный пароль");
                            $uibModalInstance.dismiss('error');
                        }
                    }, function(response) {
                        // TODO: Нужно реализовать общее окно для отображения ошибок
                        alert(response.data.errorText);
                    });
                } else {
                    authService.login($scope.loginData).then(function (response) {
                        $scope.message = "";
                        $uibModalInstance.close();
                        $state.go('home');
                    },
                    function (err) {
                        $scope.message = err;
                    });
                }
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }]
    );
}());