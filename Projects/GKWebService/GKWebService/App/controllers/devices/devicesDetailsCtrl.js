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

        	$scope.parameters = [];
        	for (var i in entity.Properties) {
        	    if (entity.Properties[i].DriverProperty.IsAUParameter) {
        	        var name = entity.Properties[i].DriverProperty.Caption;
        	        var value = entity.Properties[i].Value;
        	        if (entity.Properties[i].DriverProperty.Parameters.length === 0) {
        	            value = entity.Properties[i].DriverProperty.Multiplier > 0 ? value / entity.Properties[i].DriverProperty.Multiplier : value;
        			}
        			else {
        	            for (var j in entity.Properties[i].DriverProperty.Parameters) {
        	                if (entity.Properties[i].DriverProperty.Parameters[j].Value === entity.Properties[i].Value) {
        	                    value = entity.Properties[i].DriverProperty.Parameters[j].Name;
        					}
        				}
        			}
        	        $scope.parameters[i] = { Name: name, Value: value };
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