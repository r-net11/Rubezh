(function () {
	'use strict';
	angular.module('gkApp.controllers').controller('journalCtrl', function ($scope, $http, $uibModal, uiGridConstants, signalrJournalService) {
		var requestJournalItems = function (filter) {
			$http.post("Journal/GetJournal", filter)
				.success(function (data) {
					$scope.gridOptions.data = data;
				});
		}
		$scope.filter = null;
		requestJournalItems(null);

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
				{ name: 'Объект', field: 'Object', cellTemplate: coloredCellTemplate },
				{
					name: 'Подсистема',
					field: 'Subsystem',
					cellTemplate:
						'<div class="ui-grid-cell-contents"ng-style="!row.isSelected && {\'background-color\': row.entity.Color}">\
							<img style="vertical-align: middle; padding-right: 3px; width: 16px" ng-src="/Content/Image/Icon/SubsystemTypes/{{row.entity.SubsystemImage}}.png" />\
							{{row.entity[col.field]}}\
						</div>'
				}
			]
		};

		$scope.showFilter = function () {
			var modalInstance = $uibModal.open({
				animation: false,
				templateUrl: 'Journal/JournalFilter',
				controller: 'journalFilterCtrl',
				resolve: {
					filter: function () {
						return $scope.filter;
					}
				},
			});
			modalInstance.result.then(function (journalFilter) {
				$scope.filter = journalFilter;
				requestJournalItems(journalFilter);
			});
		};

		$scope.showSelectedRow = function (row) {
			$scope.selectedRow = row.entity;
		};

		$scope.$on('updateJournalItemsJs', function (event, args) {
			args.forEach(function (element) {
				$scope.gridOptions.data.unshift(element)
			});
			$scope.$apply();
		})
	});
}());
