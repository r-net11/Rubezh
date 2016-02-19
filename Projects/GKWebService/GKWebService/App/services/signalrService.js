(function () {
    'use strict';

    var app = angular.module('gkApp.services')
        .factory('signalrService', ['$rootScope', 'Hub', 'broadcastService', function ($rootScope, Hub, broadcastService) {
            var plansUpdater;
            var startTestBroadcast = function () {
                plansUpdater.startTestBroadcast(); //Calling a server method
            };
            
            //declaring the hub connection
            plansUpdater = new Hub('plansUpdater', {

                //client side methods
                listeners: {
                    'recieveTestMessage': function (message) {
                        $('.message').empty();
                        $('.message').append('<strong>' + htmlEncode(message)
                            + '</strong>');
                    },
                    'updateDeviceState': function (stateData) {
						broadcastService.send('updateDeviceState', stateData);
                    },
                    'updateHint': function (stateData) {
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

            if (plansUpdater.connection.state === $.signalR.connectionState.connected) {
                plansUpdater.connection.stop().start();
            }

            return {
                startTest: startTestBroadcast
            };
        }]);
}());
// This optional function html-encodes messages for display in the page.
function htmlEncode(value)
{
	var encodedValue = $('<div />').text(value).html();
	return encodedValue;
}