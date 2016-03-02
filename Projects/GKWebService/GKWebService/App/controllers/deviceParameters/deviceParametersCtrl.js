(function () {
	'use strict';

	var app = angular.module('gkApp.controllers').controller('deviceParametersCtrl',
		['$scope', '$http', '$timeout', 'uiGridTreeBaseService', 'uiGridConstants', '$uibModal', 'signalrDeviceParametersService', 'signalrDevicesService', 'broadcastService',
			function ($scope, $http, $timeout, uiGridTreeBaseService, uiGridConstants, $uibModal, signalrDeviceParametersService, signalrDevicesService, broadcastService) {

				var template = "<div class=\"ui-grid-cell-contents\"><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) ) && row.treeNode.state === 'expanded'}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<a href=\"\" ng-click=\"grid.appScope.devicesClick(row)\"><img style=\"vertical-align: middle; padding-right: 3px\" ng-src=\"/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png\"/><img style=\"vertical-align: middle\" width=\"16px\" height=\"16px\" ng-src=\"/Content/Image/{{row.entity.ImageSource}}\"/> {{row.entity[col.field]}}</a></div>";

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
						{ field: 'Smokeness', displayName: 'Задымленность, дБ/м', width: 300 },
						{ field: 'Temperature', displayName: 'Температура, C', width: 300 },
						{ field: 'Dustinness', displayName: 'Запыленность, дБ/м', width: 300 },
						{ field: 'Resistance', displayName: 'Сопротивление, Ом', width: width }
					]
				};

				$scope.expandAll = function () {
					$scope.gridApi.treeBase.expandAllRows();
				};

				$scope.toggleRow = function (row, evt) {
					uiGridTreeBaseService.toggleRowTreeState($scope.gridApi.grid, row, evt);
				};

				$scope.gridOptions.onRegisterApi = function (gridApi) {
					$scope.gridApi = gridApi;

					gridApi.selection.on.rowSelectionChanged($scope, $scope.deviceSelect);
				};

				$scope.deviceClick = function(device) {
					if (device.entity.ParentUID != undefined) {
						var modalInstance = $uibModal.open({
							animation: false,
							templateUrl: 'Devices/DeviceDetails',
							controller: 'devicesDetailsCtrl',
							size: 'rbzh',
							resolve: {
								device: function() {
									return device.entity;
								}
							}
						});
					}
				};

				function changeDevices(device) {
					for (var i = 0; i < $scope.gridOptions.data.length; i++) {
						if ($scope.gridOptions.data[i].UID === device.UID) {
							$scope.gridOptions.data[i].ImageSource = device.ImageSource;
							$scope.gridOptions.data[i].StateIcon = device.StateIcon;
							break;
						}
					}
				};

				signalrDevicesService.onDeviceChanged(function (event, args) {
				    $scope.gridState = $scope.gridApi.saveState.save();
				    changeDevices(args);
					$scope.$apply();
					$scope.gridApi.saveState.restore($scope, $scope.gridState);
				});

				$scope.deviceSelect = function (device) {
					broadcastService.send("deviceParameterSelected", device.entity);
				};

				function changeDeviceParameters(device) {
					for (var i = 0; i < $scope.gridOptions.data.length; i++) {
						if ($scope.gridOptions.data[i].UID === device.DeviceUID) {
							$scope.gridOptions.data[i].MeasureParameterValues = device.MeasureParameterValues;
							$scope.updateMeasureData($scope.gridOptions.data[i]);
							break;
						}
					}
				};

				signalrDeviceParametersService.onDeviceChanged(function (event, args) {
					changeDeviceParameters(args);
					$scope.$apply();
				});

				function getMeasureParameter(device, parameterName) {
					var items = $.grep(device.MeasureParameterValues, function (e) { return e.Name === parameterName; });
					return items.length ? items[0].StringValue : null;
				}

				$scope.updateMeasureData = function(device) {
					device.Smokeness = getMeasureParameter(device, "Задымленность, дБ/м");
					device.Temperature = getMeasureParameter(device, "Температура, C");
					device.Dustinness = getMeasureParameter(device, "Запыленность, дБ/м");
					device.Resistance = getMeasureParameter(device, "Сопротивление, Ом");
				}

				function getDeviceByUid(uid) {
					for (var i in $scope.data) {
						if ($scope.data[i].UID === uid)
							return $scope.data[i];
					}
					return null;
				};

				$http.get('DeviceParameters/GetDeviceParameters').success(function (data) {
					$scope.data = data;
					for (var i in $scope.data) {
						$scope.data[i].$$treeLevel = $scope.data[i].Level;
						$scope.data[i].ParentObject = getDeviceByUid($scope.data[i].ParentUID);

						$scope.updateMeasureData($scope.data[i]);
					}
					$scope.gridOptions.data = $scope.data;

					//Выбираем первую строку после загрузки данных
					$timeout(function () {
						if ($scope.gridApi.selection.selectRow) {
							$scope.gridApi.selection.selectRow($scope.gridOptions.data[0]);
						}
					});
				});
			}]);
}());