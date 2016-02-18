(function () {
	'use strict';

	var app = angular.module('gkApp.controllers').controller('devicesCtrl',
		['$scope', '$http', '$timeout', 'uiGridTreeBaseService', '$uibModal', '$stateParams', 'signalrDevicesService', 'broadcastService',
			function ($scope, $http, $timeout, uiGridTreeBaseService, $uibModal, $stateParams, signalrDevicesService, broadcastService) {

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

				function getDeviceByUid(uid) {
					for (var i in $scope.data) {
						if ($scope.data[i].UID === uid)
							return $scope.data[i];
					}
					return null;
				};

				$scope.deviceSelect = function(device) {
					$scope.selectedDevice = device.entity;
				};

				function changeDevices(device) {
					for (var i = 0; i < $scope.gridOptions.data.length; i++) {
						if ($scope.gridOptions.data[i].UID === device.UID) {
							$scope.gridOptions.data[i].ImageSource = device.ImageSource;
							$scope.gridOptions.data[i].StateIcon = device.StateIcon;
							break;
						}
					}

					if ($scope.selectedDevice.UID === device.UID) {
						$scope.selectedDevice = device;
					}
				};

				$scope.$on('devicesChanged', function (event, args) {
					changeDevices(args);
					$scope.$apply();
				});

				$http.get('Devices/GetDevicesList').success(function (data) {
				    if ($scope.isFull === "true") {
				        $scope.data = data;
				        for (var i in $scope.data) {
				            $scope.data[i].$$treeLevel = $scope.data[i].Level;
				            $scope.data[i].ParentObject = getDeviceByUid($scope.data[i].ParentUID);
				        }
				        $scope.gridOptions.data = $scope.data;
				        $timeout(function() {
				            $scope.expandAll();
				            if ($stateParams.uid) {
				                var device = getDeviceByUid($stateParams.uid);
				                $scope.gridApi.selection.selectRow(device);
                            }
				        });
				    }
				});

				$scope.$on('selectedZoneChanged', function (event, args) {
					if ($scope.isFull === "false") {
						$http.get('FireZones/GetDevicesByZoneUid/' + args
						).success(function (data) {
							$scope.data = data;
							for (var i in $scope.data) {
								$scope.data[i].$$treeLevel = $scope.data[i].Level;
								$scope.data[i].ParentObject = getDeviceByUid($scope.data[i].ParentUID);
							};

							$scope.gridOptions.data = $scope.data;
							$timeout(function () { $scope.expandAll(); });
						});
					}
				});
			}]);
}());