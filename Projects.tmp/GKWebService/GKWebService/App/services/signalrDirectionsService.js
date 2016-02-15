(function () {
    'use strict';

    var app = angular.module('gkApp.services')
        .factory('signalrDirectionsService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
            var directionsUpdater;

            directionsUpdater = new Hub('directionsUpdater', {

                listeners: {
                    'updateDirection': function (direction) {
                        broadcastService.send('directionChanged', direction);
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
                        break;
                    case $.signalR.connectionState.connected:
                        break;
                    case $.signalR.connectionState.reconnecting:
                        break;
                    case $.signalR.connectionState.disconnected:
                        break;
                    }
                }
            });
            return {
            };
        }]);
}());
