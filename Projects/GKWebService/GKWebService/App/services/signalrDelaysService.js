(function () {
	'use strict';

	var app = angular.module('gkApp.services')
		.factory('signalrDelaysService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
			var delaysHub = new Hub('delaysUpdaterHub', {
				listeners: {
					'delayUpdate': function (delay) {
						broadcastService.send('delayChanged', delay);
					}
				}
			});

			if (delaysHub.connection.state === $.signalR.connectionState.connected) {
			    delaysHub.connection.stop().start();
			}

			return {};
		}]);
}());