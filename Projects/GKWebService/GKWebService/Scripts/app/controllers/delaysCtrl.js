(function () {
	'use strict';
	var app = angular.module('canvasApp.controllers').controller("delaysCtrl", function ($scope, $http) {
		$http.get("home/GetDelays").success(function (data) {
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
			colNames: ['№','Задержка', 'Логика включиния', 'Задержка', 'Удержание'],
			colModel:
				[{ name: 'number', index: 'number', width: 20, sortable: false },
				{ name: 'name', index: 'name', width: Math.round(($(window).width()-320) * (1/8)), sortable: false },
				{ name: 'presentationLogic', index: 'presentationLogic', width: Math.round(($(window).width()-320) * (5/8)), sortable: false },
				{ name: 'onDelay', index: 'onDelay', width: Math.round(($(window).width()-320) * (1/8)), sortable: false },
				{ name: 'holdDelay', index: 'holdDelay', width: Math.round(($(window).width()-320) *(1/8)), sortable: false}]
		}
	});
}());