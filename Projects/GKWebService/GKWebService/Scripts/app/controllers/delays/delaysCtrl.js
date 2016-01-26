(function () {
	'use strict';
	var app = angular.module('canvasApp.controllers').controller("delaysCtrl", function ($scope, $http) {
		$http.get("Delays/GetDelays").success(function (data) {
			$scope.message = data;
			$scope.data = [];
			for (var i in data) {
				$scope.data.push({
					number: data[i].Number,
					name: data[i].Name,
					presentationLogic: data[i].PresentationLogic,
					onDelay: data[i].OnDelay,
					holdDelay: data[i].HoldDelay
				})
			}
		});
		$scope.config = {
			datatype: "local",
			height: "auto",
			colNames: ['№', 'Задержка', 'Логика включиния', 'Задержка', 'Удержание'],
			colModel:
				[{ name: 'number', index: 'number', width: 20, sortable: false },
				{ name: 'name', index: 'name', width: Math.round(($(window).width() - 320) * (1 / 8)), sortable: false },
				{ name: 'presentationLogic', index: 'presentationLogic', width: Math.round(($(window).width() - 320) * (5 / 8)), sortable: false },
				{ name: 'onDelay', index: 'onDelay', width: Math.round(($(window).width() - 320) * (1 / 8)), sortable: false },
				{ name: 'holdDelay', index: 'holdDelay', width: Math.round(($(window).width() - 320) * (1 / 8)), sortable: false }]
		}
		$scope.ButtonClick = ButtonClick;
		//function ButtonClick() {
		//	$scope.Message = "Нажали по кнопке";
		//	var delaysHub = $.connection.delaysUpdater;
		//	delaysHub.client.addNewMessageToPage = function (name, message) {
		//		alert("SignalR сработал");
		//		$scope.Message = "signarR сработал";
		//	};
		//	$.connection.hub.start().then(function () {
		//		delaysHub.server.delaysUpdate("fdfdf", "fdfd");
		//	});
		//};
		function ButtonClick() {
			$scope.Message = "Нажали по кнопке";
			$scope.data[0].number = "3";
			var connection = $.hubConnection();
			var proxy = connection.createHubProxy('delaysHub');
			proxy.on('addNewMessageToPage', function(name,message) {
				alert('signalR сработал');

			});
			connection.start().done(function () {
				proxy.invoke("delaysUpdate", "fdfd", "fdfd");
			});
			};
	});
	CreateUiGrid();
	var nameTemplate = '<p><img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png" />' +
		'<button class="btn-link" ng-click="grid.appScope.showDialogWindow()">{{row.entity.Name}}</button></p>';
function CreateUiGrid() {
	var delaysApp = angular.module("canvasApp.controllers");
	delaysApp.controller('uiGridCtrl', ['$scope', '$http', '$uibModal', function ($scope, $http, $uibModal) {
		$scope.gridOptions = {};
		$scope.enableCellEditOnFocus = true;
		$scope.gridOptions.columnDefs = [
			{ name: "Number", displayName: "№", width:40, cellTemplate:'<p><img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/Hr/Delay.png" />{{row.entity.Number}}</p>' },
			{ name: "Name", displayName: "Задержка", cellTemplate: nameTemplate },
			{ name: "PresentationLogic", displayName: "Логика включения" },
			{ name: "OnDelay", displayName: "Задержка" },
			{ name: "HoldDelay", displayName: "Удержание" }
		];

		$http.get("Delays/GetDelays").success(function (data) {
			$scope.gridOptions.data = data;
		});
		$scope.showDialogWindow = function (data) {
			alert("IsClicked");
			$uibModal.open({
				animation: false,
				templateUrl: 'Directions/DirectionDetails',
				controleer: 'delayDetailsCtrl',
				resolve: {
					direction: function () {
						return data;
					}
				},
			});
		};
		$scope.delayChange = function () {
			alert("DelayChenge()");
			$scope.gridOptions.data[0].OnDelay = "3";
		};
	}]);
};
}());
