(function () {
    'use strict';

    var app = angular.module('canvasApp.services')
        .factory('signalrMPTsService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
            //declaring the hub connection
           var mptsUpdater = new Hub('mptHub', {
                //client side methods
                listeners: {
                    'mptStateUpdate': function (mpt) {
                        broadcastService.send('mptChanged', mpt);
                    }
                },
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
           return [];
        }]);
}());
