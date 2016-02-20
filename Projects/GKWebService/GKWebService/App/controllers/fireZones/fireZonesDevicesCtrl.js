(function () {

	'use strict';

	var app = angular.module('gkApp.controllers').controller('fireZonesDevicesCtrl',
        ['$scope', '$http', '$stateParams', 'uiGridConstants', 'signalrFireZonesDevicesService', 'broadcastService', 'dialogService', 'constants',
        function ($scope, $http, $stateParams, uiGridConstants, signalrFireZonesDevicesService, broadcastService, dialogService, constants) {

	        var template = "<div class=\"ui-grid-cell-contents\"><a href=\"#\" ng-click=\"grid.appScope.devicesClick(row)\"><img style=\"vertical-align: middle; padding-right: 3px\" ng-src=\"/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png\"/> <img style=\"vertical-align: middle\" width=\"16px\" height=\"16px\" ng-src=\"/Content/Image/{{row.entity.ImageSource}}\"/> {{row.entity[col.field]}}</a></div>";
        	var width = $(window).width() - 625;
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
					{ field: 'Name', width: 350, displayName: 'Устройство', cellTemplate: template },
					{ field: 'Address', displayName: 'Адрес', width: 100 },
					{ field: 'Description', displayName: 'Примечание', width: width }
        		]
        	};

        	function getDeviceByUid(uid) {
        		for (var i in $scope.data) {
        			if ($scope.data[i].UID === uid)
        				return $scope.data[i];
        		}
        	};

        	$scope.gridStyle = function () {
        		var ctrlHeight = (window.innerHeight - 100) / 2;
        		return "height:" + ctrlHeight + "px";
        	}();


        	$scope.$on('selectedZoneChanged', function (event, args) {
        		$http.get('FireZones/GetDevicesByZoneUid/' + args
		        ).success(function (data) {
		        	$scope.data = data;
		        	for (var i in $scope.data) {
		        		$scope.data[i].ParentObject = getDeviceByUid($scope.data[i].ParentUID);
		        	};

		        	$scope.gridOptions.data = $scope.data;
		        });
        	});

        	$scope.devicesClick = function (device) {
        		dialogService.showWindow(constants.gkObject.device, device.entity);
        	};

        	function changeDevices(device) {
        		for (var i = 0; i < $scope.gridOptions.data.length; i++) {
        			if ($scope.gridOptions.data[i].UID === device.UID) {
        				$scope.gridOptions.data[i].StateIcon = device.StateIcon;
        				break;
        			}
        		}
        	};

        	$scope.$on('devicesChanged', function (event, args) {
        		changeDevices(args);
        		$scope.$apply();
        	});
        }]
    );
}());
