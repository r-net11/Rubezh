(function () {
	'use strict';

	angular.module("gkApp").directive('devicesGrid', function () {

		var template = "<div class=\"ui-grid-cell-contents\"><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) ) && row.treeNode.state === 'expanded'}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<a href=\"\" ng-click=\"grid.appScope.devicesClick(row)\"><img style=\"vertical-align: middle; padding-right: 3px\" ng-src=\"/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png\"/><img style=\"vertical-align: middle\" width=\"16px\" height=\"16px\" ng-src=\"/Content/Image/{{row.entity.ImageSource}}\"/> {{row.entity[col.field]}}</a></div>";

		return {
			link: function (scope, element, attrs) {
				scope.onClick = attrs['onClick'];
				scope.onSelect = attrs['onSelect'];
			},
			controller: function ($scope, $attrs, uiGridTreeBaseService, uiGridConstants) {
				var width = $(window).width() - 925;
				$scope.gridOptions = {
					enableSorting: false,
					enableFiltering: false,
					showTreeExpandNoChildren: false,
					multiSelect: false,
					enableRowHeaderSelection: false,
					noUnselect: true,
					enableColumnMenus: false,
					showTreeRowHeader: false,
					enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
					columnDefs: [
						{ field: 'Name', width: 350, displayName: 'Устройство', cellTemplate: template },
						{ field: 'Address', displayName: 'Адрес', width: 100 },
						{ field: 'PresentationZone', displayName: 'Зона или логика', width: 300 },
						{ field: 'Description', displayName: 'Примечание', width: width }
					]
				};

				$scope.gridStyle = function () {
					var ctrlHeight = window.innerHeight - 215;
					return "height:" + ctrlHeight + "px";
				}();

				$scope.expandAll = function () {
					$scope.gridApi.treeBase.expandAllRows();
				};

				$scope.toggleRow = function (row, evt) {
					uiGridTreeBaseService.toggleRowTreeState($scope.gridApi.grid, row, evt);
				};

				$scope.gridOptions.onRegisterApi = function (gridApi) {
					$scope.gridApi = gridApi;

					if ($scope.onSelect) {
						gridApi.selection.on.rowSelectionChanged($scope, $scope[$scope.onSelect]);
					}
				};

				$scope.devicesClick = function (device) {
					$scope[$scope.onClick](device);
				};
			},
			restrict: 'E',
			transclude: false,
			template: '<div ui-grid="gridOptions" style="{{gridStyle}}" ui-grid-tree-view ui-grid-selection ui-grid-resize-columns></div>'
		};
	});
}());