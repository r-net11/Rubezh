(function () {

	angular.module('gkApp.controllers').controller('devicesDetailsCtrl',
        function ($scope, $http, uiGridConstants, $uibModalInstance, device) {
        	$scope.device = device;

        	$scope.$on('devicesChanged', function (event, args) {
        		if (args.UID === $scope.device.UID) {
        			$scope.device = args;
        			$scope.$apply();
        		};
        	});

        	var tmp = '<div align="center" style="color: black; font-weight: bold">{{row.entity[col.field]}}</div>';

        	function gridConfig(data, colDefs) {
        		var config = {};
        		config.data = data;
        		config.enableRowHeaderSelection = false;
        		config.enableSorting = false;
        		config.multiSelect = false;
        		config.enableColumnMenus = false;
        		config.enableVerticalScrollbar = uiGridConstants.scrollbars.NEVER;
        		config.enableHorizontalScrollbar = uiGridConstants.scrollbars.NEVER;
        		config.rowHeight = 35;
        		config.columnDefs = colDefs;
        		return config;
        	}

        	$scope.gridMeasurements = gridConfig(device.MeasureParameters, [
					{ field: 'Name', displayName: 'Измерение', cellTemplate: tmp }
        	]);

        	var parameters = [];
        	for (var i in device.Properties) {
        		if (device.Properties[i].DriverProperty.IsAUParameter) {
        			var name = device.Properties[i].DriverProperty.Caption;
        			var value = device.Properties[i].Value;
        			if (device.Properties[i].DriverProperty.Parameters.length === 0) {
        				value = device.Properties[i].DriverProperty.Multiplier > 0 ? value / device.Properties[i].DriverProperty.Multiplier : value;
        			}
        			else {
        				for (var j in device.Properties[i].DriverProperty.Parameters) {
        					if (device.Properties[i].DriverProperty.Parameters[j].Value === device.Properties[i].Value) {
        						value = device.Properties[i].DriverProperty.Parameters[j].Name;
        					}
        				}
        			}
        			parameters[i] = { Name: name, Value: value };
        		}
        	};

        	$scope.gridParameters = gridConfig(parameters, [
					{ field: 'Name', displayName: 'Параметр', cellTemplate: tmp },
					{ field: 'Value', displayName: 'Значение', cellTemplate: tmp }
        	]);

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

        	};

        	$scope.ShowJournal = function () {

        	};

        	$scope.ok = function () {
        		$uibModalInstance.close();
        	};

        	$scope.cancel = function () {
        		$uibModalInstance.dismiss('cancel');
        	};
        }
    );
}());