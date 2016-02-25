(function () {
	var app = angular.module('gkApp.services')
	.factory('journalService', function () {
		return {
			createGridOptions: function (scope) {
				scope.showSelectedRow = function (row) {
					scope.selectedRow = row.entity;
				};

				var coloredCellTemplate =
				'<div ng-style="!row.isSelected && {\'background-color\': row.entity.Color}" class="ui-grid-cell-contents">\
							{{row.entity[col.field]}}\
						</div>';

				return {
					enableRowSelection: true,
					enableRowHeaderSelection: false,
					multiSelect: false,
					modifierKeysToMultiSelect: true,
					noUnselect: true,
					enableSorting: false,
					enableColumnResizing: true,
					enableColumnMenus: false,
					onRegisterApi: function (gridApi) {
						scope.gridApi = gridApi;
						gridApi.selection.on.rowSelectionChanged(scope, scope.showSelectedRow);
					},
					columnDefs: [
						{ name: 'Дата в системе', width: '8%', field: 'SystemDate', cellTemplate: coloredCellTemplate },
						{ name: 'Дата в приборе', width: '8%', field: 'DeviceDate', cellTemplate: coloredCellTemplate },
						{ name: 'Название', field: 'Name', cellTemplate: coloredCellTemplate },
						{ name: 'Уточнение', field: 'Desc', cellTemplate: coloredCellTemplate },
						{
							name: 'Объект',
							cellTemplate:
								'<div class="ui-grid-cell-contents" ng-style="!row.isSelected && {\'background-color\': row.entity.Color}">\
								<img style="vertical-align: middle; padding-right: 3px; width: 16px" ng-src="{{row.entity.ObjectImageSource}}" />\
								 <a href="#" ng-click="grid.appScope.showPropertiesClick(row.entity)">\
									{{row.entity.ObjectName}}\
								</a>\
							</div>'
						},
					]
				};
			}
		}
	});
}());