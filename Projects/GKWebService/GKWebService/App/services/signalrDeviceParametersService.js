(function () {

	var app = angular.module('gkApp.services')
	.factory('signalrDeviceParametersService', ['Hub', 'broadcastService', function (Hub, broadcastService) {

		var devicesHub = new Hub('DeviceParametersUpdater', {
			listeners: {
				'deviceParameterUpdate': function (device) {
					broadcastService.send('signalrDeviceParametersService.deviceParameterChanged', device);
				}
			}
		});

		if (devicesHub.connection.state === $.signalR.connectionState.connected) {
			devicesHub.connection.stop().start();
		}

		return {
			onDeviceChanged: function (fnc) {
				broadcastService.on('signalrDeviceParametersService.deviceParameterChanged', fnc);
			}
		};
	}]);
}());