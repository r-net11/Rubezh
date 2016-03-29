(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('authService', ['$q', '$http', '$window', '$uibModal', function ($q, $http, $window, $uibModal) {
        var authServiceFactory = {};

        authServiceFactory.authentication = {
            isAuth: false,
            userName: "",
            permissions: [],
            checkPermission: function (permission) {
                return authServiceFactory.authentication.permissions.indexOf(permission) !== -1;
            }
        };

        authServiceFactory.login = function (loginData) {

            var deferred = $q.defer();

            $http.post("Home/Login", loginData).success(function (response) {

                if (response.success) {
                    $http.get("Home/GetCurrentUserPermissions").then(function (responsePermissions) {
                        authServiceFactory.authentication.isAuth = true;
                        authServiceFactory.authentication.userName = loginData.userName;
                        authServiceFactory.authentication.permissions = responsePermissions.data.permissions;
                        deferred.resolve(response);
                    });

                } else {
                    deferred.reject(response.message);
                }
            }).error(function (err, status) {
                deferred.reject(err);
            });

            return deferred.promise;

        };

        authServiceFactory.logOut = function () {
            $http.post("Home/LogOut").then(function(response) {

                authServiceFactory.authentication.isAuth = false;
                authServiceFactory.authentication.userName = '';
                authServiceFactory.authentication.permissions = [];

                $window.location.reload();
            });
        };

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

        authServiceFactory.checkPermission = function (permission) {
            return authServiceFactory.authentication.permissions.indexOf(permission) !== -1;
        };

        authServiceFactory.сonfirm = function () {
            var deferred = $q.defer();

            if (authServiceFactory.checkPermission('Oper_MayNotConfirmCommands')) {
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
                    deferred.resolve({ headers: {'Password': password}});
                }, function () {
                    deferred.reject();
                });
            }

            return deferred.promise;
        };

        return authServiceFactory;
    }]);
}());
