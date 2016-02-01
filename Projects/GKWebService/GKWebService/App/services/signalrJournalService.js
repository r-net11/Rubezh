(function () {
	'use strict';

	var app = angular.module('gkApp.services')
		.factory('signalrJournalService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
			console.log('signalrJournalService');
			var delaysHub = new Hub('journalUpdaterHub', {
				listeners: {
					'updateJournalItems': function (journalItems) {
						console.log('broadcastService.send');
						broadcastService.send('updateJournalItemsJs', journalItems);
					}
				}
			});
			return {};
		}]);
}());