(function () {
    'use strict';

    var app = angular.module('canvasApp.services', ['SignalR'])
        .factory('signalrDirectionsService', ['Hub', function (Hub) {
            var directionsUpdater;
            var startTestBroadcast1 = function () {
                directionsUpdater.startTestBroadcast1(); //Calling a server method
            };
            
            //declaring the hub connection
            directionsUpdater = new Hub('directionsUpdater', {

                //client side methods
                listeners: {
                    'updateDirection': function (direction) {
                        console.info ("directionsUpdater");
                    }
                },

                //server side methods
                methods: ['startTestBroadcast1'],

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
                        startTestBroadcast1();
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
                startTest: startTestBroadcast1
            };
        }]);
}());
