(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('authService', ['$q', '$http', '$state', function ($q, $http, $state) {
        var authServiceFactory = {};

        authServiceFactory.authentication = {
            isAuth: false,
            userName: ""
        };

        authServiceFactory.login = function (loginData) {

            var deferred = $q.defer();

            $http.post("Home/Login", loginData).success(function (response) {

                if (response.success) {
                    authServiceFactory.authentication.isAuth = true;
                    authServiceFactory.authentication.userName = loginData.userName;

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

                $state.go('login');
            });
        };

        authServiceFactory.fillAuthData = function () {
            $http.get("Home/TryGetCurrentUserName").then(function (response) {
                authServiceFactory.authentication.isAuth = true;
                authServiceFactory.authentication.userName = response.data.userName;
            });
        };

        return authServiceFactory;
    }]);
}());
