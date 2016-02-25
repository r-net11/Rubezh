(function () {
	'use strict';

	angular.module('gkApp.controllers').controller('journalCtrl', function ($scope, $http, $timeout, uiGridConstants, $uibModal, signalrJournalService, broadcastService, journalService) {
		var requestJournalItems = function (filter) {
			$http.post("Journal/GetJournal", filter)
				.success(function (data) {
					$scope.gridOptions.data = data;
				});
		}
		$scope.filter = null;
		requestJournalItems(null);

		$scope.gridOptions = journalService.createGridOptions($scope);
		
		$scope.gridStyle = function () {
			var ctrlHeight = window.innerHeight - 270;
			return "height:" + ctrlHeight + "px";
		}();

		$scope.showFilter = function () {
			var modalInstance = $uibModal.open({
				animation: false,
				templateUrl: 'Archive/ArchiveFilter',
				controller: 'archiveFilterCtrl',
				resolve: {
					filter: function () {
						return $scope.filter;
					},
					isArchive: function () {
						return false;
					},
				},
			});
			modalInstance.result.then(function (journalFilter) {
				$scope.filter = journalFilter;
				requestJournalItems(journalFilter);
			});
		};

		$scope.$on('updateJournalItemsJs', function (event, args) {
			args.forEach(function (element) {
				$scope.gridOptions.data.unshift(element)
			});
			$scope.$apply();
		})
	});
}());
