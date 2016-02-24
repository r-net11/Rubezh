(function () {
	'use strict';

	var app = angular.module('gkApp.services')
        .factory('signalrConfigService', ['Hub', function (Hub) {

        	var configUpdater = new Hub('configUpdater',
			{
				logging: true,
				listeners: {
					'configUpdate': function () {
						location.reload();
					}
				}
			});
        	if (configUpdater.connection.state === $.signalR.connectionState.connected) {
        		configUpdater.connection.stop().start();
        	}

        	return {};
        }]);
}());
