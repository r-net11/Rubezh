(function () {
	'use strict';

	var app = angular.module('gkApp.controllers').controller('devicesCtrl',
		['$scope', '$http', '$timeout', 'uiGridTreeBaseService', 'broadcastService', function ($scope, $http, $timeout, uiGridTreeBaseService, broadcastService) {

			var template = "<div class=\"ui-grid-cell-contents\"><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) ) && row.treeNode.state === 'expanded'}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<a href=\"#\" ng-click=\"grid.appScope.fireZonesClick(row.entity)\"><img style=\"vertical-align: middle; padding-right: 3px\" ng-src=\"/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png\"/><img style=\"vertical-align: middle\" width=\"16px\" height=\"16px\" ng-src=\"/Content/Image/{{row.entity.ImageSource}}\"/> {{row.entity[col.field]}}</a></div>";

			$scope.gridOptions = {
				enableSorting: false,
				enableFiltering: false,
				showTreeExpandNoChildren: false,
				multiSelect: false,
				enableRowHeaderSelection: false,
				noUnselect: true,
				enableColumnMenus: false,
				showTreeRowHeader: false,
				columnDefs: [
					{ field: 'Name', width: 300, displayName: 'Устройство', cellTemplate: template },
					{ field: 'Address', displayName: 'Адрес', width: 100 },
					{ field: 'Description', displayName: 'Примечание', width: $(window).width() - 650 }
				]
			};

			$http.get('Devices/GetDevicesList').success(function (data, status, headers, config) {

				$scope.data = data;

				for (var i in $scope.data) {
					$scope.data[i].$$treeLevel = $scope.data[i].Level;
				}

				$scope.gridOptions.data = $scope.data;

				$timeout(function () {
					$scope.expandAll();
				});
			});

			function changeDevices(device) {
				for (var i = 0; i < $scope.gridOptions.data.length; i++) {
					if ($scope.gridOptions.data[i].UID === device.UID) {
						$scope.gridOptions.data[i].ImageSource = device.ImageSource;
						$scope.gridOptions.data[i].StateIcon = device.StateIcon;
						break;
					}
				}
			};

			$scope.$on('devicesChanged', function (event, args) {
				changeDevices(args);
				$scope.$apply();
			});

			$scope.expandAll = function () {
				$scope.gridApi.treeBase.expandAllRows();
			};

			$scope.toggleRow = function (row, evt) {
				uiGridTreeBaseService.toggleRowTreeState($scope.gridApi.grid, row, evt);
			};

			$scope.gridOptions.onRegisterApi = function (gridApi) {
				$scope.gridApi = gridApi;
				gridApi.selection.on.rowSelectionChanged($scope, $scope.deviceSelected);
			};

			$scope.devicesClick = function (device) {
				console.debug(device);
			};

			$scope.deviceSelected = function (device) {

				var selectedDevice = device.isSelected ? device.entity : null;
				broadcastService.send("selectedDeviceChanged", selectedDevice);

				$scope.selectedDevice = selectedDevice;
			};

			$scope.findDevice = function (deviceId) {

				for (var i in $scope.gridOptions.data) {
					if ($scope.gridOptions.data[i].UID === deviceId)
						return $scope.gridOptions.data[i];
				}

				return null;
			};

			$scope.$on('showGKDevice', function (event, args) {
				for (var i = 0; i < $scope.gridOptions.data.length; i++) {
					if ($scope.gridOptions.data[i].UID === args) {
						$scope.gridApi.selection.selectRow($scope.gridOptions.data[i]);
						break;
					}
				}
			});
		}]);
}());