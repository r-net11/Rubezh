(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('authService', ['$q', '$http', '$window', '$uibModal', function ($q, $http, $window, $uibModal) {
        var authServiceFactory = {};

        authServiceFactory.authentication = {
            isAuth: false,
            userName: "",
            permissions: []
        };

        authServiceFactory.login = function (loginData) {

            var deferred = $q.defer();

            $http.post("Home/Login", loginData).success(function (response) {

                if (response.success) {
                    authServiceFactory.authentication.isAuth = true;
                    authServiceFactory.authentication.userName = loginData.userName;
                    $http.get("Home/GetCurrentUserPermissions").then(function (responsePermissions) {
                        authServiceFactory.authentication.permissions = responsePermissions.data.permissions;
                    });

                    deferred.resolve(response);
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
            $http.get("Home/TryGetCurrentUserName").then(function (response) {
                authServiceFactory.authentication.isAuth = true;
                authServiceFactory.authentication.userName = response.data.userName;
                $http.get("Home/GetCurrentUserPermissions").then(function(responsePermissions) {
                    authServiceFactory.authentication.permissions = responsePermissions.data.permissions;
                });
            });
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

                modalInstance.result.then(function () {
                    deferred.resolve();
                }, function () {
                    deferred.reject();
                });
            }

            return deferred.promise;
        };

        return authServiceFactory;
    }]);
}());
