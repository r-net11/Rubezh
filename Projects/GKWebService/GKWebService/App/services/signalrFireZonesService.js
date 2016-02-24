(function () {
	'use strict';

	var app = angular.module('gkApp.services')
        .factory('signalrFireZonesService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
        	var fireZonesUpdater = new Hub('fireZonesUpdater', {
        		logging: true,
        		listeners: {
        			'updateFireZone': function (data) {
        				broadcastService.send('fireZonesChanged', data);
        			}
        		}
        	});
        	if (fireZonesUpdater.connection.state === $.signalR.connectionState.connected) {
        		fireZonesUpdater.connection.stop().start();
        	}
        	return {};
        }]);
}());
