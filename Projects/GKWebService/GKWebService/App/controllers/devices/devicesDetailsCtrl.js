(function () {

	angular.module('gkApp.controllers').controller('devicesDetailsCtrl',
        ['$scope', '$http', 'uiGridConstants', '$uibModalInstance', '$state', 'signalrDevicesService', 'entity',
        function ($scope, $http, uiGridConstants, $uibModalInstance, $state, signalrDevicesService, entity) {
        	$scope.device = entity;

        	$scope.$on('devicesChanged', function (event, args) {
        		if (args.UID === $scope.device.UID) {
        			$scope.device = args;
        			$scope.$apply();
        		};
        	});

        	$scope.device.parameters = [];
        	for (var i in $scope.device.Properties) {
        	    if ($scope.device.Properties[i].DriverProperty.IsAUParameter) {
        	        var name = $scope.device.Properties[i].DriverProperty.Caption;
        	        var value = $scope.device.Properties[i].Value;
        	        if ($scope.device.Properties[i].DriverProperty.Parameters.length === 0) {
        	            value = $scope.device.Properties[i].DriverProperty.Multiplier > 0 ? value / $scope.device.Properties[i].DriverProperty.Multiplier : value;
        			}
        			else {
        	            for (var j in $scope.device.Properties[i].DriverProperty.Parameters) {
        	                if ($scope.device.Properties[i].DriverProperty.Parameters[j].Value === $scope.device.Properties[i].Value) {
        	                    value = $scope.device.Properties[i].DriverProperty.Parameters[j].Name;
        					}
        				}
        			}
        	        $scope.device.parameters[i] = { Name: name, Value: value };
        		}
        	};

        	
        	$scope.SetIgnoreState = function () {
        		$http.post('Devices/SetIgnoreState', { id: $scope.device.UID });
        	};

        	$scope.SetAutomaticState = function () {
        		$http.post('Devices/SetAutomaticState', { id: $scope.device.UID });
        	};

        	$scope.Reset = function () {
        		$http.post('Devices/Reset', { id: $scope.device.UID });
        	};

        	$scope.SetManualState = function () {
        		$http.post('Devices/SetManualState', { id: $scope.device.UID });
        	};

        	$scope.TurnOn = function () {
        		$http.post('Devices/TurnOn', { id: $scope.device.UID });
        	};

        	$scope.TurnOnNow = function () {
        		$http.post('Devices/TurnOnNow', { id: $scope.device.UID });
        	};

        	$scope.ForbidStart = function () {
        		$http.post('Devices/ForbidStart', { id: $scope.device.UID });
        	};

        	$scope.TurnOff = function () {
        		$http.post('Devices/TurnOff', { id: $scope.device.UID });
        	};

        	$scope.Show = function () {
        	    $state.go('device', { uid: $scope.device.UID });
        	};

        	$scope.ShowJournal = function () {
        	    $state.go('archive', { uid: $scope.device.UID });
        	};

        	$scope.ok = function () {
        		$uibModalInstance.close();
        	};

        	$scope.cancel = function () {
        		$uibModalInstance.dismiss('cancel');
        	};
        }]
    );
}());