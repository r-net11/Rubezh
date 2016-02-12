(function () {
	'use strict';

	var app = angular.module('gkApp.controllers').controller('fireZonesDevicesCtrl',
		['$scope', '$http', '$timeout', 'uiGridTreeBaseService', '$uibModal', 'broadcastService', function ($scope, $http, $timeout, uiGridTreeBaseService, $uibModal, broadcastService) {

			$scope.deviceClick = function (device) {
				var modalInstance = $uibModal.open({
					animation: false,
					templateUrl: 'Devices/DeviceDetails',
					controller: 'devicesDetailsCtrl',
					size: 'rbzh',
					resolve: {
						device: function () {
							return device.entity;
						}
					}
				});
			}

			$scope.deviceSelect = function (device) {
				$scope.selectedDevice = device.entity;
			}

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

			$scope.$on('selectedZoneChanged', function (event, args) {
				$http.get('FireZones/GetDevicesByZoneUID/' + args
	            ).success(function (data, status, headers, config) {
	            	$scope.data = [];
	            	for (var i in data) {
	            		$scope.data[i] = data[i].DeviceList[0];
	            	};
	            	for (var i in $scope.data) {
	            		$scope.data[i].$$treeLevel = data.length - $scope.data[i].Level - 1;
	            	};


	            	$scope.gridOptions.data = $scope.data;
	            	//Раскрываем дерево после загрузки
	            	$timeout(function () {
	            		$scope.expandAll();
	            	});
	            });
			});
		}]);
}());