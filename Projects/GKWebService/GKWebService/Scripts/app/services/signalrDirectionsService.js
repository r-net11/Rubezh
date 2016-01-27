(function () {
    'use strict';

    var app = angular.module('canvasApp.services')
        .factory('signalrDirectionsService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
            var directionsUpdater;
            var startTestBroadcast = function () {
                directionsUpdater.startTestBroadcast(); //Calling a server method
            };

            //declaring the hub connection
            directionsUpdater = new Hub('directionsUpdater', {

                //client side methods
                listeners: {
                    'updateDirection': function (direction) {
                        broadcastService.send('directionChanged', direction);
                    }
                },

                //server side methods
                methods: ['startTestBroadcast'],

                //query params sent on initial connection
                queryParams: {
                    'token': 'exampletoken'
                },

                //handle connection error
                errorHandler: function (error) {
                    console.error(error);
                },

                //specify a non default root
                //rootPath: '/api

                stateChanged: function (state) {
                    switch (state.newState) {
                    case $.signalR.connectionState.connecting:
                        //your code here
                        break;
                    case $.signalR.connectionState.connected:
                        startTestBroadcast();
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
            return {
                startTest: startTestBroadcast
            };
        }]);
}());
