(function () {
    angular.module('gkApp.controllers').controller('journalFilterCtrl',
        function ($scope, $http, $uibModal, $uibModalInstance, $timeout, filter, uiGridTreeBaseService) {
        	$scope.toggleRow = function (gridApi, row, evt) {
        		uiGridTreeBaseService.toggleRowTreeState(gridApi.grid, row, evt);
        	};

        	$scope.beginDate = {
        		date: new Date(),
        		isOpened: false,
        		open: function () {
        			$scope.beginDate.isOpened = true;
        		}
        	}

        	$scope.endDate = {
        		date: new Date(),
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
						<img style=\"vertical-align: middle; padding-right: 3px\" width=\"16px\" height=\"16px\" ng-src=\"Content/Image/{{row.entity.ImageSource}}\"/>\
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
        		columnDefs: [{ name: 'Объект', width: 500, cellTemplate: objectsNameTemplate }],
        		onRegisterApi: function (gridApi) {
        			$scope.objectsGridApi = gridApi;
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
						<img style=\"vertical-align: middle; padding-right: 3px\" width=\"16px\" height=\"16px\" ng-src=\"Content/Image/{{row.entity.ImageSource}}\"/>\
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
        		}
        	};

        	$http.get('Journal/GetFilter')
				.success(function (data) {
					$scope.minDate = data.MinDate;
					$scope.maxDate = data.MaxDate;
					data.Objects.forEach(function (item) {
						item.$$treeLevel = item.Level;
					})
					$scope.objectsGrid.data = data.Objects;
					$timeout(function () {
						if (filter && filter.ObjectUids)
							for (var i in $scope.objectsGrid.data) {
								var row = $scope.objectsGrid.data[i];
								for (var j in filter.ObjectUids) {
									if (row.UID == filter.ObjectUids[j])
										$scope.objectsGridApi.selection.selectRow(row);
								}
							}
					}, 100);
					data.Events.forEach(function (item) {
						item.$$treeLevel = item.Level;
					})
					$scope.eventsGrid.data = data.Events;
					$timeout(function () {
						if (filter && filter.Events)
							for (var i in $scope.eventsGrid.data) {
								var row = $scope.eventsGrid.data[i];
								for (var j in filter.Events) {
									if (row.Type == filter.Events[j].Type && row.Value == filter.Events[j].Value) {
										$scope.eventsGridApi.selection.selectRow(row);
									}
								}
							}
					}, 100);
				});
        	

        	$scope.ok = function () {
        		$uibModalInstance.close(createFilter());
        	};

        	$scope.cancel = function () {
        		$uibModalInstance.dismiss('cancel');
        	};

        	var createFilter = function() {
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
        });
}());