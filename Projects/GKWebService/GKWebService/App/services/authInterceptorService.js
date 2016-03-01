(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('authInterceptorService', ['$q', '$injector', function ($q, $injector) {
        var authInterceptorServiceFactory = {};

        authInterceptorServiceFactory.responseError = function (rejection) {
            if (rejection.status === 401) {
                var state = $injector.get('$state');
                state.go('login');
            }
            return $q.reject(rejection);
        }

        return authInterceptorServiceFactory;
    }]);
}());
