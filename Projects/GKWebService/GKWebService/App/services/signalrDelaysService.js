﻿(function () {
	'use strict';

	var app = angular.module('canvasApp.services')
		.factory('signalrDelaysService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
			var delaysHub = new Hub('delaysUpdaterHub', {
				listeners: {
					'delayUpdate': function (delay) {
						broadcastService.send('delayChanged', delay);
					}
				}
			});
			return {};
		}]);
}());