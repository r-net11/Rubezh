(function () {
	angular.module('gkApp.controllers').controller('archiveFilterCtrl',
        function ($scope, $http, $uibModal, $uibModalInstance, $timeout, filter, uiGridTreeBaseService) {
        	$scope.toggleRow = function (gridApi, row, evt) {
        		uiGridTreeBaseService.toggleRowTreeState(gridApi.grid, row, evt);
        	};

        	$scope.beginDate = {
        		date: filter && filter.BeginDate ? filter.BeginDate : (function () {
        			var date = new Date();
        			date.setDate(date.getDate() - 7);
        			return date;
        		}()),
        		isOpened: false,
        		open: function () {
        			$scope.beginDate.isOpened = true;
        		}
        	}

        	$scope.endDate = {
        		date: filter && filter.EndDate ? filter.EndDate : new Date(),
        		isOpened: false,
        		open: function () {
        			$scope.endDate.isOpened = true;
        		}
        	}

        	var objectsNameTemplate =
				"<div class=\"ui-grid-cell-contents\">\
        			<div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" \
        				ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" \
        				ng-click=\"grid.appScope.toggleRow(grid.appScope.objectsGridApi, row,evt)\">\
        					<i ng-class=\"{\
        							'ui-grid-icon-minus-squared': ((grid.options.showTreeExpandNoChildren && row.treeLevel > -1) || (row.treeNode.children && row.treeNode.children.length > 0))\
										&& row.treeNode.state === 'expanded', \
        							'ui-grid-icon-plus-squared': ((grid.options.showTreeExpandNoChildren && row.treeLevel > -1) || (row.treeNode.children && row.treeNode.children.length > 0))\
										&& row.treeNode.state === 'collapsed' \
        						}\" \
        						ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + (row.treeNode.children && row.treeNode.children.length > 0 ? 0 : 2 * grid.options.treeIndent) + 'px'}\">\
        					</i> &nbsp;\
        			</div>\
					<div ng-style=\"{'color': 'black'}\">\
						<img class=\"treeImage\" ng-src=\"Content/Image/{{row.entity.ImageSource}}\"/>\
        				{{row.entity.Name}}\
        			</div>\
        		</div>";

        	$scope.objectsGrid = {
        		enableSorting: false,
        		enableFiltering: false,
        		showTreeExpandNoChildren: false,
        		enableColumnMenus: false,
        		showTreeRowHeader: false,
        		enableRowSelection: true,
        		enableSelectAll: true,
        		multiSelect: true,
        		columnDefs: [{ name: 'Объект', width: '100%', cellTemplate: objectsNameTemplate }],
        		onRegisterApi: function (gridApi) {
        			$scope.objectsGridApi = gridApi;
        			gridApi.selection.on.rowSelectionChanged($scope, $scope.objectsSetChildren);
        		}
        	};

        	var eventsNameTemplate =
				"<div class=\"ui-grid-cell-contents\">\
        			<div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" \
        				ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" \
        				ng-click=\"grid.appScope.toggleRow(grid.appScope.eventsGridApi, row,evt)\">\
        					<i ng-class=\"{\
        							'ui-grid-icon-minus-squared': ((grid.options.showTreeExpandNoChildren && row.treeLevel > -1) || (row.treeNode.children && row.treeNode.children.length > 0))\
										&& row.treeNode.state === 'expanded', \
        							'ui-grid-icon-plus-squared': ((grid.options.showTreeExpandNoChildren && row.treeLevel > -1) || (row.treeNode.children && row.treeNode.children.length > 0))\
										&& row.treeNode.state === 'collapsed' \
        						}\" \
        						ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + (row.treeNode.children && row.treeNode.children.length > 0 ? 0 : 2 * grid.options.treeIndent) + 'px'}\">\
        					</i> &nbsp;\
        			</div>\
					<div ng-style=\"{'color': 'black'}\">\
						<img class=\"treeImage\" ng-src=\"Content/Image/{{row.entity.ImageSource}}\"/>\
        				{{row.entity.Name}}\
        			</div>\
        		</div>";

        	$scope.eventsGrid = {
        		enableSorting: false,
        		enableFiltering: false,
        		showTreeExpandNoChildren: false,
        		enableColumnMenus: false,
        		showTreeRowHeader: false,
        		enableRowSelection: true,
        		enableSelectAll: true,
        		multiSelect: true,
        		columnDefs: [{ name: 'Событие', width: 500, cellTemplate: eventsNameTemplate }],
        		onRegisterApi: function (gridApi) {
        			$scope.eventsGridApi = gridApi;
        			gridApi.selection.on.rowSelectionChanged($scope, $scope.eventsSetChildren);
        		}
        	};

        	$http.get('Archive/GetFilter')
				.success(function (data) {
					$scope.minDate = data.MinDate;
					$scope.maxDate = data.MaxDate;
					data.Objects.forEach(function (item) {
						item.$$treeLevel = item.Level;
					})
					$scope.objectsGrid.data = data.Objects;
					data.Events.forEach(function (item) {
						item.$$treeLevel = item.Level;
					})
					$scope.eventsGrid.data = data.Events;
					$timeout(function () {
						if (filter && filter.ObjectUids)
							for (var i in $scope.objectsGrid.data) {
								var row = $scope.objectsGrid.data[i];
								for (var j in filter.ObjectUids) {
									if (row.UID == filter.ObjectUids[j])
										$scope.objectsGridApi.selection.selectRow(row);
								}
							}
						if (filter && filter.Events)
							for (var i in $scope.eventsGrid.data) {
								var row = $scope.eventsGrid.data[i];
								for (var j in filter.Events) {
									if (row.Type == filter.Events[j].Type && row.Value == filter.Events[j].Value)
										$scope.eventsGridApi.selection.selectRow(row);
								}
							}
					}, 100);
				});


        	$scope.ok = function () {
        		$uibModalInstance.close(createFilter());
        	};

        	$scope.cancel = function () {
        		$uibModalInstance.dismiss();
        	};

        	$scope.reset = function () {
        		$scope.objectsGridApi.selection.clearSelectedRows();
        		$scope.eventsGridApi.selection.clearSelectedRows();
        		$scope.endDate.date = new Date();
        		$scope.beginDate.date = new Date();
        		$scope.beginDate.date.setDate($scope.beginDate.date.getDate() - 7);
        	};

        	var createFilter = function () {
        		var filter = {};
        		var objects = $scope.objectsGridApi.selection.getSelectedRows();
        		filter.ObjectUids = [];
        		objects.forEach(function (item) {
        			filter.ObjectUids.push(item.UID);
        		});
        		var events = $scope.eventsGridApi.selection.getSelectedRows();
        		filter.Events = [];
        		events.forEach(function (item) {
        			filter.Events.push({
        				Type: item.Type,
        				Value: item.Value,
        			});
        		});
        		$scope.beginDate.date.setSeconds(0, 0);
        		filter.BeginDate = $scope.beginDate.date;
        		$scope.endDate.date.setSeconds(0, 0);
        		filter.EndDate = $scope.endDate.date;
        		return filter;
        	}

        	$scope.objectsSetChildren = function (row) {
        		showSelectedRow(row, $scope.objectsGrid, $scope.objectsGridApi);
        	};

        	$scope.eventsSetChildren = function (row) {
        		showSelectedRow(row, $scope.eventsGrid, $scope.eventsGridApi);
        	};

        	var showSelectedRow = function (row, gridOptions, gridApi) {
        		var index = gridOptions.data.indexOf(row.entity) + 1;
        		var item = gridOptions.data[index];
        		if (!item)
        			return;
        		while (item.Level > row.entity.Level) {
        			if (row.isSelected)
        				gridApi.selection.selectRow(item);
        			else
        				gridApi.selection.unSelectRow(item);
        			index++;
        			item = gridOptions.data[index];
        		}
        	};
        });
}());