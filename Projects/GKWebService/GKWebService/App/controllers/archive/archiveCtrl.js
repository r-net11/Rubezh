(function () {
	'use strict';
	angular.module('gkApp.controllers').controller('archiveCtrl', function ($scope, $http, $uibModal, uiGridConstants, signalrJournalService) {
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
		$scope.filter = {};
		$scope.MaxPage = 1;
		requestMaxPage($scope.filter);
		setPage(1);
		
		$scope.gridOptions = {
			enableRowSelection: true,
			enableRowHeaderSelection: false,
			multiSelect: false,
			modifierKeysToMultiSelect: true,
			noUnselect: true,
			enableSorting: false,
			enableColumnResizing: true,
			enableColumnMenus: false,
			onRegisterApi: function(gridApi) {
				$scope.gridApi = gridApi;
				gridApi.selection.on.rowSelectionChanged($scope, $scope.showSelectedRow);
			},
			columnDefs: [
				{ name: 'Дата в системе', field: 'SystemDate' },
				{ name: 'Дата в приборе', field: 'DeviceDate' },
				{ name: 'Название', field: 'Name' },
				{ name: 'Уточнение', field: 'Desc' },
				{ name: 'Объект', field: 'Object' },
				{
					name: 'Подсистема',
					cellTemplate:
						'<div class="ui-grid-cell-contents">\
							<img style="vertical-align: middle; padding-right: 3px; width: 16px" ng-src="/Content/Image/Icon/SubsystemTypes/{{row.entity.SubsystemImage}}.png" />\
							{{row.entity.Subsystem}}\
						</div>'
				}
			]
		};

		$scope.test = function () {
			getByUid($scope.gridOptions.data[0].ObjectUid);
		}

		$scope.showFilter = function () {
			var modalInstance = $uibModal.open({
				animation: false,
				templateUrl: 'Archive/ArchiveFilter',
				controller: 'archiveFilterCtrl',
				resolve: {
					filter: function () {
						return $scope.filter;
					}
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

		$scope.showSelectedRow = function () {
			$scope.selectedRow = $scope.gridApi.selection.getSelectedRows()[0]
		};
	});
}());
