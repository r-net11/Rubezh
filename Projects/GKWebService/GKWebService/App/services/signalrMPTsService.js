(function () {
    'use strict';

    var app = angular.module('gkApp.services')
        .factory('signalrMPTsService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
            var mptsUpdater = new Hub('mptsUpdater', {
                listeners: {
                    'mptUpdate': function (mpt) {
                        broadcastService.send('mptChanged', mpt);
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

            if (mptsUpdater.connection.state === $.signalR.connectionState.connected) {
                mptsUpdater.connection.stop().start();
            }

            return {};
        }]);
}());
