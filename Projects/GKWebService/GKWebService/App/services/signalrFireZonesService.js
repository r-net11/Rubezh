(function () {
    'use strict';

    var app = angular.module('gkApp.services')
        .factory('signalrFireZonesService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
            var fireZonesUpdater = new Hub('fireZonesUpdater', {
                listeners: {
                    'updateFireZone': function (data) {
                        broadcastService.send('fireZonesChanged', data);
                    }
                },
                errorHandler: function (error) {
                    console.error(error);
                }
            });
            return {
            };
        }]);
}());
