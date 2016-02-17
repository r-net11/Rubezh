(function () {
	'use strict';

	var app = angular.module('gkApp.services')
		.factory('signalrJournalService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
			var journalHub = new Hub('journalUpdaterHub', {
				listeners: {
					'updateJournalItems': function (journalItems) {
						broadcastService.send('updateJournalItemsJs', journalItems);
					}
				}
			});

			if (journalHub.connection.state === $.signalR.connectionState.connected) {
				journalHub.connection.stop().start();
			}

			return {};
		}]);
}());