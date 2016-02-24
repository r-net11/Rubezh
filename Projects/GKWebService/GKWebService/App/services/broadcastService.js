(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('broadcastService', function ($rootScope) {
        return {
            send: function (msg, data) {
                $rootScope.$broadcast(msg, data);
            },
            on: function (msg, fnc) {
            	$rootScope.$on(msg, fnc);
            }
        }
    });
}());
