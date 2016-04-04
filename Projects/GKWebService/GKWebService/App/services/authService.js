(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('authService', ['$q', '$http', '$window', '$uibModal', function ($q, $http, $window, $uibModal) {
        var authServiceFactory = {};

        // объект с данными текущего пользователя
        authServiceFactory.authentication = {
            isAuth: false,
            userName: "",
            permissions: [],
            checkPermission: function (permission) {
                return this.permissions.indexOf(permission) !== -1;
            }
        };

        // аудентификация
        authServiceFactory.login = function (loginData) {

            var deferred = $q.defer();

            $http.post("Home/Login", loginData).then(function (response) {

                if (response.data.success) {
                    // если пользователь успешно аудентифицирован, то получаем его список прав
                    $http.get("Home/GetCurrentUserPermissions").then(function (responsePermissions) {
                        authServiceFactory.authentication.isAuth = true;
                        authServiceFactory.authentication.userName = loginData.userName;
                        authServiceFactory.authentication.permissions = responsePermissions.data.permissions;
                        deferred.resolve(response);
                    });

                } else {
                    deferred.reject(response.data.message);
                }
            }, function (response) {
                deferred.reject(response);
            });

            return deferred.promise;

        };

        // выход пользователя
        authServiceFactory.logOut = function () {
            $http.post("Home/LogOut").then(function(response) {

                authServiceFactory.authentication.isAuth = false;
                authServiceFactory.authentication.userName = '';
                authServiceFactory.authentication.permissions = [];

                $window.location.reload();
            });
        };

        // получение информации о текущем пользователе при старте приложения
        authServiceFactory.fillAuthData = function () {
            var deferred = $q.defer();

            if (authServiceFactory.authentication.isAuth) {
                deferred.resolve(authServiceFactory.authentication);
            } else {
                var userName;
                $http.get("Home/TryGetCurrentUserName")
                    .then(function (response) {
                        userName = response.data.userName;
                        return $http.get("Home/GetCurrentUserPermissions");
                    })
                    .then(function (responsePermissions) {
                        authServiceFactory.authentication.isAuth = true;
                        authServiceFactory.authentication.userName = userName;
                        authServiceFactory.authentication.permissions = responsePermissions.data.permissions;
                        deferred.resolve(authServiceFactory.authentication);
                    })
                    .catch(function (err) {
                        deferred.reject(err);
                    });
            }

            return deferred.promise;
        };

        // проверка наличия прав у пользователя
        authServiceFactory.checkPermission = function (permission) {
            return authServiceFactory.authentication.permissions.indexOf(permission) !== -1;
        };

        // подтверждение пароля пользователя, при выполнении пользователем важных операций
        authServiceFactory.сonfirm = function () {
            var deferred = $q.defer();

            if (authServiceFactory.checkPermission('Oper_MayNotConfirmCommands')) {
                // если пользователя имеет право не подтвержать пароль, то не подтверждать пароль
                deferred.resolve();
            } else {
                var modalInstance = $uibModal.open({
                    templateUrl: '/Home/Login',
                    controller: 'loginCtrl',
                    backdrop: 'static',
                    size: 'rbzh',
                    keyboard: false,
                    resolve: {
                        options: function () {
                            return {
                                validateOnlyPassword: true
                            }
                        }
                    }
                });

                modalInstance.result.then(function (password) {
                    // отправка на сервер важной команды вместе с паролем
                    deferred.resolve({ headers: { 'Password': password } });
                }, function () {
                    deferred.reject();
                });
            }

            return deferred.promise;
        };

        return authServiceFactory;
    }]);
}());
