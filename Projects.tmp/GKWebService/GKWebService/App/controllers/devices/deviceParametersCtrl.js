(function () {
	'use strict';

	var PropertyTypeEnum =
	{
		EnumType: 0,
		StringType: 1,
		IntType: 2,
		BoolType: 3
	};


	var app = angular.module('gkApp.controllers').controller('deviceParametersCtrl',
		['$scope', '$http', 'broadcastService',
		function ($scope, $http, broadcastService) {

			$scope.$on('selectedDeviceChanged', function (event, device) {

				$scope.selectedDevice = device;
				$scope.initDeviceProperties();
			});

			$scope.initDeviceProperties = function () {

				$scope.stringProperties = [];
				$scope.enumProperties = [];
				$scope.shortProperties = [];
				$scope.boolProperties = [];

				$scope.selectedDevice.Properties.forEach(function (property) {

					if (property.DriverProperty.CanNotEdit)
						return;

					switch (property.DriverProperty.DriverPropertyType) {

						case PropertyTypeEnum.StringType:
							$scope.stringProperties.push(property);
							break;

						case PropertyTypeEnum.EnumType:

							property.DeviceAUParameterValue = $scope.findParameter(property, property.Value).Name;
							$scope.enumProperties.push(property);
							break;

						case PropertyTypeEnum.IntType:

							property.DeviceAUParameterValue = property.Value;

							if (property.DriverProperty.Multiplier !== 0)
								property.DeviceAUParameterValue = property.Value / property.DriverProperty.Multiplier;

							$scope.shortProperties.push(property);
							break;

						case PropertyTypeEnum.BoolType:

							$scope.boolProperties.push(property);
							break;
					}
				});
			};

			$scope.findProperty = function (propertyName) {

				for (var i = 0; i < $scope.selectedDevice.Properties.length; i++) {
					if ($scope.selectedDevice.Properties[i].DriverProperty.Name === propertyName)
						return $scope.selectedDevice.Properties[i];
				}

				return null;
			};

			$scope.findParameter = function (property, parameterValue) {

				for (var i = 0; i < property.DriverProperty.Parameters.length; i++) {
					if (property.DriverProperty.Parameters[i].Value === parameterValue)
						return property.DriverProperty.Parameters[i];
				}

				return null;
			};

		}]);
}());