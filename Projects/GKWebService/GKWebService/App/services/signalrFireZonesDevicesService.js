(function () {

	var app = angular.module('gkApp.services')
	.factory('signalrFireZonesDevicesService', ['Hub', 'broadcastService', function (Hub, broadcastService) {

		var devicesHub = new Hub('devicesUpdater', {
			logging: true,
			listeners: {
				'devicesUpdate': function (device) {
					broadcastService.send('devicesChanged', device);
				}
			}
		});

		if (devicesHub.connection.state === $.signalR.connectionState.connected) {
			devicesHub.connection.stop().start();
		}

		return {};
	}]);
}());