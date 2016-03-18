(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('authService', ['$q', '$http', '$window', function ($q, $http, $window) {
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
                    authServiceFactory.authentication.permissions = response.data.permissions;

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
                authServiceFactory.authentication.permissions = response.data.permissions;
            });
        };

        authServiceFactory.checkPermission = function (permission) {
            return authServiceFactory.authentication.permissions.indexOf(permission) !== -1;
        };

        return authServiceFactory;
    }]);
}());
