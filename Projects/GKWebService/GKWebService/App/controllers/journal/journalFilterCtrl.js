(function () {
    angular.module('gkApp.controllers').controller('journalFilterCtrl',
        function ($scope, $http, $uibModal, $uibModalInstance, $timeout, filter, uiGridTreeBaseService) {
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
						<img style=\"vertical-align: middle; padding-right: 3px\" width=\"16px\" height=\"16px\" ng-src=\"Content/Image/{{row.entity.ImageSource}}\"/>\
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
					if (filter != null && filter.deviceuids != null)
						$timeout(function () {
							$scope.gridapi.treebase.expandallrows();
							for (var i in $scope.gridoptions.data) {
								var row = $scope.gridoptions.data[i];
								for (var j in filter.deviceuids) {
									if (row.uid == filter.deviceuids[j])
										$scope.gridapi.selection.selectrow(row);
								}
							}
						}, 100);
				});

        	$scope.toggleRow = function (row, evt) {
        		uiGridTreeBaseService.toggleRowTreeState($scope.gridApi.grid, row, evt);
        	};

        	$scope.ok = function () {
        		$uibModalInstance.close(createFilter());
        	};

        	$scope.cancel = function () {
        		$uibModalInstance.dismiss('cancel');
        	};

        	var createFilter = function()
        	{
        		var filter = {};
				var devices = $scope.gridApi.selection.getSelectedRows();
				filter.deviceUids = [];
				devices.forEach(function (item) {
					filter.deviceUids.push(item.Uid);
				});
				return filter;
			}
        });
}());