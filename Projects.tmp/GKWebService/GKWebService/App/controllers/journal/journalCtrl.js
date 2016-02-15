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

		$scope.showSelectedRow = function () {
			$scope.selectedRow = $scope.gridApi.selection.getSelectedRows()[0]
		};

		$scope.$on('updateJournalItemsJs', function (event, args) {
			args.forEach(function (element) {
				$scope.gridOptions.data.unshift(element)
			});
			$scope.$apply();
		})
	});
}());
