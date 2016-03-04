(function () {

	angular.module('gkApp.services')
	.factory('signalrDevicesService', ['Hub', 'broadcastService', function (Hub, broadcastService) {

		var devicesHub = new Hub('devicesUpdater', {
			//useSharedConnection: false,
			logging: true,
			listeners: {
				'devicesUpdate': function (device) {
					broadcastService.send('signalrDevicesService.devicesChanged', device);
				}
			}
		});

		if (devicesHub.connection.state === $.signalR.connectionState.connected) {
			devicesHub.connection.stop().start();
		}

		return {
			onDeviceChanged: function (fnc) {
				broadcastService.on('signalrDevicesService.devicesChanged', fnc);
			}
		};
	}]);
}());