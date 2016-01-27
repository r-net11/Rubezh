(function () {
	'use strict';
	var nameTemplate = '<p><img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" />' +
		'<button class="btn-link" ng-click="grid.appScope.showDialogWindow()">{{row.entity.Name}}</button></p>';

	var delaysApp = angular.module("canvasApp.controllers");
	delaysApp.controller('delaysCtrl', ['$scope', '$http', '$uibModal', function ($scope, $http, $uibModal) {
		function CreateConnection() {
			var connection = $.hubConnection();
			var proxy = connection.createHubProxy('delaysHub');
			proxy.on('createInstance', function () {
				alert('Хаб создан');
			});
			proxy.on('delayStateUpdate', StateIconUpdate);
			connection.start().done(function () {
				proxy.invoke("createInstance");
			});
		};
		CreateConnection();
		function GetDelays() {
			$http.get("Delays/GetDelays").success(function (data) {
				$scope.gridOptions.data = data;
			});
		};
		function StateIconUpdate(delayUid,stateIcon) {
			GetDelays();
		};
		$scope.gridOptions = {};
		$scope.enableCellEditOnFocus = true;
		$scope.gridOptions.enableSorting = false;
		$scope.gridOptions.columnDefs = [
			{ name: "Number", displayName: "№", width:40, cellTemplate:'<p><img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Delay.png" />{{row.entity.Number}}</p>' },
			{ name: "Name", displayName: "Задержка", cellTemplate: nameTemplate },
			{ name: "PresentationLogic", displayName: "Логика включения" },
			{ name: "OnDelay", displayName: "Задержка" },
			{ name: "HoldDelay", displayName: "Удержание" }
		];

		GetDelays();
		$scope.showDialogWindow = function (data) {
			alert("IsClicked");
			$uibModal.open({
				animation: false,
				templateUrl: 'Delays/DelayDetails',
				controleer: 'delayDetailsCtrl',
				resolve: {
					direction: function () {
						return data;
					}
				},
			});
		};
	}]);
}());
