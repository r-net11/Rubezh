(function () {
	'use strict';
	angular.module('gkApp.controllers').controller('archiveCtrl', function ($scope, $http, $uibModal, $stateParams, journalService) {
		var requestJournalItems = function (filter) {
			$http.post("Archive/GetArchive", filter)
				.success(function (data) {
					$scope.gridOptions.data = data;
					$scope.isLoading = false;
				});
		}
		var requestMaxPage = function (filter) {
			$http.post("Archive/GetMaxPage", filter)
				.success(function (maxPage) {
					$scope.MaxPage = maxPage;
				});
		}
		var setPage = function (page) {
			$scope.filter.Page = page;
			$scope.isLoading = true;
			requestJournalItems($scope.filter);
		}
		var getByUid = function (uid) {
			$scope.isLoading = false;
			$scope.filter = {};
			$scope.filter.ObjectUids = [];
			$scope.filter.ObjectUids.push(uid);
			$scope.filter.endDate = new Date();
			$scope.filter.beginDate = new Date();
			$scope.filter.beginDate.setDate($scope.filter.beginDate.getDate() - 7);
			requestMaxPage($scope.filter);
			setPage(1);
		}

		if ($stateParams.uid) {
		    getByUid($stateParams.uid);
		} else {
		    $scope.filter = {};
		    $scope.filter.endDate = new Date();
		    $scope.filter.beginDate = new Date();
		    $scope.filter.beginDate.setDate($scope.filter.beginDate.getDate() - 7);
		    requestMaxPage($scope.filter);
		    setPage(1);
		}

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
						return true;
					},
				},
			});
			modalInstance.result.then(function (journalFilter) {
				$scope.filter = journalFilter;
				$scope.filter.Page = 1;
				$scope.isLoading = true;
				requestJournalItems($scope.filter);
				requestMaxPage($scope.filter);
			});
		};

		$scope.nextPage = function () {
			if ($scope.filter.Page < $scope.MaxPage) {
				setPage($scope.filter.Page + 1);
			}
		}

		$scope.previousPage = function () {
			if ($scope.filter.Page > 1) {
				setPage($scope.filter.Page - 1);
			}
		}

		$scope.firstPage = function () {
			setPage(1);
		}

		$scope.lastPage = function () {
			setPage($scope.MaxPage);
		}

		$scope.$on('showArchive', function (event, args) {
		    getByUid(args);
		});

		$scope.pageNumberChanged = function () {
			if (!$scope.filter.Page)
				$scope.filter.Page = 1;
			if ($scope.filter.Page > $scope.MaxPage)
				$scope.filter.Page = $scope.MaxPage;
			if ($scope.filter.Page < 1)
				$scope.filter.Page = 1;
			setPage($scope.filter.Page);
		};
	});
}());
