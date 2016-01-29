(function () {
    'use strict';

    var app = angular.module('gkApp.services')
        .factory('signalrFireZonesService', ['Hub', function (Hub) {
            var fireZonesUpdater;

            var startStatesMonitoring = function () {
                fireZonesUpdater.StartStatesMonitoring(); //Calling a server method
            };

            fireZonesUpdater = new Hub('fireZonesUpdater', {
                //client side methods
                listeners: {
                    'RefreshZoneState': function (data) {
                        for (var i in data) {
                            $('td:nth-child(2) > img:nth-child(2)')[i].src = "data:image/gif;base64," + data[i].StateImageSource.Item1;
                        }
                    }
                },

                //server side methods
                methods: ['StartStatesMonitoring'],

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
                            startStatesMonitoring();
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
                startMonitoring: startStatesMonitoring
            };
        }]);
}());
