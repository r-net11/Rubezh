(function () {
	'use strict';

	var app = angular.module('gkApp.controllers').controller('devicesCtrl',
		['$scope', '$http', '$timeout', 'uiGridTreeBaseService', '$uibModal', 'broadcastService', function ($scope, $http, $timeout, uiGridTreeBaseService, $uibModal, broadcastService) {

			$scope.deviceClick = function(device) {
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
		}]);
}());