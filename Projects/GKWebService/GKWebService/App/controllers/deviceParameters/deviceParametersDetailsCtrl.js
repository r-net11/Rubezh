(function () {
	'use strict';

	var app = angular.module('gkApp.controllers').controller('deviceParametersDetailsCtrl',
		['$scope', '$http', '$timeout', 'uiGridTreeBaseService', 'uiGridConstants', '$uibModal', 'signalrDeviceParametersService', 'signalrDevicesService', 'broadcastService',
			function ($scope, $http, $timeout, uiGridTreeBaseService, uiGridConstants, $uibModal, signalrDeviceParametersService, signalrDevicesService, broadcastService) {

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
						{ field: 'Name', width: 350, displayName: 'Параметр' },
						{ field: 'StringValue', displayName: 'Значение'}
					]
				};

				broadcastService.on("deviceParameterSelected", function (event, args) {
					$scope.selectedDeviceUID = args.UID;
					$scope.gridOptions.data = args.MeasureParameterValues;
				});

				$scope.gridOptions.onRegisterApi = function (gridApi) {
					$scope.gridApi = gridApi;
				};

				signalrDeviceParametersService.onDeviceChanged(function (event, args) {
					if($scope.selectedDeviceUID === args.DeviceUID) {
						$scope.gridOptions.data = args.MeasureParameterValues;
					}
				});
			}]);
}());