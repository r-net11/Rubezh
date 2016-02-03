(function () {
    angular.module('gkApp.controllers').controller('journalFilterCtrl',
        function ($scope, $http, $uibModal, journalProperties, uiGridTreeViewConstants, uiGridTreeBaseService) {
        	var nameTemplate =
				"<div class=\"ui-grid-cell-contents\">\
        			<div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" \
        				ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" \
        				ng-click=\"grid.appScope.toggleRow(row,evt)\">\
        					<i ng-class=\"{\
        							'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', \
        							'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed' \
        						}\" \
        						ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + (row.treeNode.children && row.treeNode.children.length > 0 ? 0 : 2 * grid.options.treeIndent) + 'px'}\">\
        					</i> &nbsp;\
        			</div>\
        			<a href=\"#\">\
						<img style=\"vertical-align: middle; padding-right: 3px\" width=\"16px\" height=\"16px\" ng-src=\"data:image/gif;base64,{{row.entity.ImageDeviceIcon.Item1}}\"/>\
        				{{row.entity[col.field]}}\
        			</a>\
        		</div>";
        	$scope.gridOptions = {
        		enableSorting: false,
        		enableFiltering: false,
        		showTreeExpandNoChildren: false,
        		enableColumnMenus: false,
        		showTreeRowHeader: false,
        		enableRowSelection: true,
        		enableSelectAll: true,
        		multiSelect: true,
        		columnDefs: [
					{ field: 'Name', width: 300, displayName: 'Устройство', cellTemplate: nameTemplate },
					{ field: 'Address', displayName: 'Адрес', width: 100 },
        		],
        		onRegisterApi: function (gridApi) {
        			$scope.gridApi = gridApi;
        			gridApi.selection.on.rowSelectionChanged($scope, $scope.showSelectedRow);
        			gridApi.selection.on.rowSelectionChangedBatch($scope, $scope.showSelectedRow);
        		}
        	};

        	$scope.showSelectedRow = function () {
        		$scope.selectedRow = $scope.gridApi.selection.getSelectedRows()
        	};

        	$http.get('Journal/GetFilterDevices')
			.success(function (data) {
				data.forEach(function (item) {
					item.$$treeLevel = item.Level;
				})
				$scope.gridOptions.data = data;
			});

        	$scope.expandAll = function () {
        		$scope.gridApi.treeBase.expandAllRows();
        	};

        	$scope.toggleRow = function (row, evt) {
        		uiGridTreeBaseService.toggleRowTreeState($scope.gridApi.grid, row, evt);
        	};
        });
}());