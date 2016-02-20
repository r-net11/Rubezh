(function () {
	'use strict';
	angular.module('gkApp.controllers').controller('archiveCtrl', function ($scope, $http, $uibModal, $stateParams, uiGridConstants) {
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

		var coloredCellTemplate =
			'<div ng-style="!row.isSelected && {\'background-color\': row.entity.Color}" class="ui-grid-cell-contents">\
				{{row.entity[col.field]}}\
			</div>';

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
				{ name: 'Дата в системе', field: 'SystemDate', cellTemplate: coloredCellTemplate },
				{ name: 'Дата в приборе', field: 'DeviceDate', cellTemplate: coloredCellTemplate },
				{ name: 'Название', field: 'Name', cellTemplate: coloredCellTemplate },
				{ name: 'Уточнение', field: 'Desc', cellTemplate: coloredCellTemplate },
				{
					name: 'Объект',
					cellTemplate:
						'<div class="ui-grid-cell-contents" ng-style="!row.isSelected && {\'background-color\': row.entity.Color}">\
							<img style="vertical-align: middle; padding-right: 3px; width: 16px" ng-src="{{row.entity.ObjectImageSource}}" />\
							{{row.entity.ObjectName}}\
						</div>'
				},
				{
					name: 'Подсистема',
					cellTemplate:
						'<div class="ui-grid-cell-contents" ng-style="!row.isSelected && {\'background-color\': row.entity.Color}">\
							<img style="vertical-align: middle; padding-right: 3px; width: 16px" ng-src="/Content/Image/Icon/SubsystemTypes/{{row.entity.SubsystemImage}}.png" />\
							{{row.entity.Subsystem}}\
						</div>'
				}
			]
		};

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

		$scope.showSelectedRow = function (row) {
			$scope.selectedRow = row.entity;
		};

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
