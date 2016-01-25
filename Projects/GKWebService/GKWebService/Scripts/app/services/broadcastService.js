(function () {
    'use strict';

    var app = angular.module('canvasApp.services');
    app.factory('broadcastService', function ($rootScope) {
        return {
            send: function (msg, data) {
                $rootScope.$broadcast(msg, data);
            }
        }
    });
}());
