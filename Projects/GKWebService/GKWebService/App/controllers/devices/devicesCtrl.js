(function () {

	'use strict';

	angular.module('gkApp.controllers').controller('devicesCtrl',
	['$scope', '$http', '$timeout', 'uiGridTreeBaseService', '$uibModal', '$stateParams', 'signalrDevicesService', 'broadcastService', 'dialogService', 'constants',
		function ($scope, $http, $timeout, uiGridTreeBaseService, $uibModal, $stateParams, signalrDevicesService, broadcastService, dialogService, constants) {

			$scope.deviceClick = function (device) {
				if (device.entity.ParentUID != undefined) {
					dialogService.showWindow(constants.gkObject.device, device.entity);
				}
			};

			function getDeviceByUid(uid) {
				for (var i in $scope.data) {
					if ($scope.data[i].UID === uid)
						return $scope.data[i];
				}
			};

			$scope.deviceSelect = function (device) {
				$scope.selectedDevice = device.entity;
			};

			function changeDevices(device) {
				for (var i = 0; i < $scope.gridOptions.data.length; i++) {
					if ($scope.gridOptions.data[i].UID === device.UID) {
						$scope.gridOptions.data[i].StateIcon = device.StateIcon;
						break;
					}
				}

				if ($scope.selectedDevice && $scope.selectedDevice.UID === device.UID) {
					$scope.selectedDevice = device;
				}
			};

			signalrDevicesService.onDeviceChanged(function (event, args) {
				changeDevices(args);
				$scope.$apply();
			});

			$http.get('Devices/GetDevicesList').success(function (data) {
				$scope.data = data;
				for (var i in $scope.data) {
					$scope.data[i].$$treeLevel = $scope.data[i].Level;
					$scope.data[i].ParentObject = getDeviceByUid($scope.data[i].ParentUID);

				}
				$scope.gridOptions.data = $scope.data;
				$timeout(function () {
					$scope.expandAll();
					if ($stateParams.uid) {
						var device = getDeviceByUid($stateParams.uid);
						$scope.gridApi.selection.selectRow(device);
						$timeout(function () {
							$scope.gridApi.core.scrollTo(device, $scope.gridOptions.columnDefs[0]);
						});
					}
				});
			});
		}]);
}());