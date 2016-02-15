(function () {
    'use strict';

    var app = angular.module('gkApp.services')
        .factory('signalrGuardZonesService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
            var guardZonesUpdater = new Hub('guardZonesUpdater', {
                listeners: {
                    'guardZoneUpdate': function (guardZone) {
                        broadcastService.send('guardZoneChanged', guardZone);
                    }
                },
                queryParams: {
                    'token': 'exampletoken'
                },
                errorHandler: function (error) {
                    console.error(error);
                },

                stateChanged: function (state) {
                    switch (state.newState) {
                        case $.signalR.connectionState.connecting:
                            //your code here
                            break;
                        case $.signalR.connectionState.connected:
                            break;
                        case $.signalR.connectionState.reconnecting:
                            //your code here
                            break;
                        case $.signalR.connectionState.disconnected:
                            //your code here
                            break;
                    }
                }
            });
            return {};
        }]);
}());
