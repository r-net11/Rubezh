(function () {
	'use strict';

	var app = angular.module('gkApp.services')
        .factory('signalrConfigService', ['Hub', 'dialogService', 'constants', function (Hub, dialogService, constants) {

        	var configUpdater = new Hub('configUpdater',
			{
				logging: true,
				listeners: {
					'configUpdate': function () {
						dialogService.showWindow(constants.gkObject.restart);
					}
				}
			});
        	if (configUpdater.connection.state === $.signalR.connectionState.connected) {
        		configUpdater.connection.stop().start();
        	}

        	return {};
        }]);
}());
