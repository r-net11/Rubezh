(function () {
	'use strict';
	angular.module('gkApp.services')
        .factory('signalrFireZonesService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
        	var fireZonesUpdater = new Hub('fireZonesUpdater', {
        		logging: true,
        		listeners: {
        			'updateFireZone': function (data) {
        				broadcastService.send('fireZonesChanged', data);
        			}
        		}
        	});
        	var devicesHub = new Hub('devicesUpdater', {
        		logging: true,
        		listeners: {
        			'devicesUpdate': function (device) {
        				broadcastService.send('signalrFireZonesService.devicesChanged', device);
        			}
        		}
        	});
        	if (fireZonesUpdater.connection.state === $.signalR.connectionState.connected) {
        		fireZonesUpdater.connection.stop().start();
        	}
        	if (devicesHub.connection.state === $.signalR.connectionState.connected) {
        		devicesHub.connection.stop().start();
        	}

        	return {
        		onDeviceChanged: function (fnc) {
        			broadcastService.on('signalrFireZonesService.devicesChanged', fnc);
        		}
        	};
        }]);
}());
