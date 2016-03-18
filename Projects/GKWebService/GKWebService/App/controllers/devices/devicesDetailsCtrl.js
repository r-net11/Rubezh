(function () {

	angular.module('gkApp.controllers').controller('devicesDetailsCtrl',
        ['$scope', '$http', '$timeout', 'uiGridConstants', '$uibModalInstance', '$state', 'signalrDevicesService', 'entity', 'authService',
        function ($scope, $http, $timeout, uiGridConstants, $uibModalInstance, $state, signalrDevicesService, entity, authService) {
        	$scope.device = entity;

        	$scope.oper_Device_Control = authService.checkPermission('Oper_Device_Control');

        	$scope.$on('signalrDevicesService.devicesChanged', function (event, args) {
        		if (args.UID === $scope.device.UID) {
        			$scope.device = args;
        			$scope.$apply();
        		};
        	});

        	var tmp = '<div align="center" style="color: black; font-weight: bold">{{row.entity[col.field]}}</div>';

        	function gridConfig(data, colDefs) {
        		var config = {};
        		config.data = [];
        		for (var i in data) {
        			if (!data[i].IsNotVisible) {
				        config.data.push(data[i]);
			        }
		        }
        		config.enableRowHeaderSelection = false;
        		config.enableSorting = false;
        		config.multiSelect = false;
        		config.enableColumnMenus = false;
        		config.enableHorizontalScrollbar = uiGridConstants.scrollbars.NEVER;
        		config.rowHeight = 35;
        		config.columnDefs = colDefs;
        		return config;
        	}

        	$scope.gridMeasurements = gridConfig($scope.device.MeasureParameters, [
					{ field: 'Name', displayName: 'Параметр', cellTemplate: tmp },
					{ field: 'Value', displayName: 'Значение', cellTemplate: tmp }
        	]);

        	var parameters = [];

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
        			parameters[i] = { Name: name, Value: value };
        		}
        	};

        	$scope.gridParameters = gridConfig(parameters, [
					{ field: 'Name', displayName: 'Параметр', cellTemplate: tmp },
					{ field: 'Value', displayName: 'Значение', cellTemplate: tmp }
        	]);

        	$scope.onTabSelected = function () {
        		$timeout(function () {
        			$(window).resize();
        		});
        	}

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

        	$scope.ShowJournal = function () {
        		$state.go('archive', { uid: $scope.device.UID });
        	};
			
        	$scope.ShowDevice = function () {
        		$state.go('device', { uid: $scope.device.UID });
        	};

        	$scope.ShowParentDevice = function () {
        		$state.go('device', { uid: $scope.device.ParentUID });
        	};

        	$scope.ShowZone = function () {
        		$state.go('fireZones', { uid: $scope.device.ZoneUID });
        	};

        	$scope.ShowOnPlan = function (planUID) {
		        var deviceUID = $scope.device.UID;
		        $state.go('plan', { uid: planUID });
	        };

        	$scope.ok = function () {
        		$uibModalInstance.close();
        	};

        	$scope.onExecuteCommand = function (command) {
        		$http.post('Devices/OnExecuteCommand', { commandName: command, UID: $scope.device.UID });
	        }

        	$scope.cancel = function () {
        		$uibModalInstance.dismiss('cancel');
        	};
        }]
    );
}());