(function () {
	'use strict';
	var nameTemplate = '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" />' +
		'<a href="#" ng-click="grid.appScope.showDetailsDelay(row.entity)">{{row.entity.Name}}</a></div>';

	var delaysApp = angular.module("gkApp.controllers");
	delaysApp.controller('delaysCtrl', ['$scope', '$http', '$uibModal', '$stateParams', '$timeout', 'signalrDelaysService', 'dialogService', 'constants',
	    function ($scope, $http, $uibModal, $stateParams, $timeout, signalrDelaysService, dialogService, constants) {
			function ChangeDelay(delay)
			{
				for (var i = 0; i < $scope.gridOptions.data.length; i++) {
					if ($scope.gridOptions.data[i].Uid == delay.Uid) {
						$scope.gridOptions.data[i] = delay;
					}
				}
			}
			$scope.gridOptions = {
				enableSorting: false,
				enableColumnResizing: true,
				enableColumnMenus: false,
				onRegisterApi: function (gridApi) {
				    $scope.gridApi = gridApi;
				},
				columnDefs: [
				{ name: "Number", displayName: "№", width: 40, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Delay.png" />{{row.entity.Number}}</div>' },
				{ name: "Name", displayName: "Задержка", width: 300, cellTemplate: nameTemplate },
				{ name: "PresentationLogic", displayName: "Логика включения" },
				{ name: "OnDelay", displayName: "Задержка", width: 120 },
				{ name: "HoldDelay", displayName: "Удержание", width: 120 }
				]
			};

			$scope.gridStyle = function () {
				var ctrlHeight = window.innerHeight - 100;
				return "height:" + ctrlHeight + "px";
			}();

			$http.get("Delays/GetDelays").success(function (data) {
				$scope.gridOptions.data = data;
				$timeout(function () {
				    if ($stateParams.uid) {
				        $scope.selectRowById($stateParams.uid);
				    } else {
				        if ($scope.gridApi.selection.selectRow) {
				            $scope.gridApi.selection.selectRow($scope.gridOptions.data[0]);
				        }
				    }
				});
			});

			$scope.showDetailsDelay = function (delay) {
			    dialogService.showWindow(constants.gkObject.delay, delay);
			};
		    $scope.$on('delayChanged', function(event, args) {
		        ChangeDelay(args);
		        $scope.$apply();
		    });

		    $scope.selectRowById = function (uid) {
		        for (var i = 0; i < $scope.gridOptions.data.length; i++) {
		            if ($scope.gridOptions.data[i].Uid === uid) {
			            $scope.gridApi.selection.selectRow($scope.gridOptions.data[i]);
			            break;
			        }
			    }
			};
		}]);
}());
