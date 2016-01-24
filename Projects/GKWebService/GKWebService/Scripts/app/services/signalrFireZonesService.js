(function () {
    'use strict';

    var app = angular.module('canvasApp.services')
        .factory('signalrFireZonesService', ['Hub', function (Hub) {
            var fireZonesUpdater;

            var startTestBroadcast = function () {
                fireZonesUpdater.startTestBroadcast(); //Calling a server method
            };

            fireZonesUpdater = new Hub('fireZonesUpdater', {
                //client side methods
                listeners: {
                    'RefreshZoneState': function (imageBloom) {
                        $('td:nth-child(2) > img:nth-child(2)')[0].src = "data:image/gif;base64," + imageBloom;
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
