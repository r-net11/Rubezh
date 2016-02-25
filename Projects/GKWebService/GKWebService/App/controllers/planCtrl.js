(function() {
	"use strict";

	var app = angular.module("gkApp.controllers").controller("PlanCtrl", [
		"$scope", "plansListFactory", "dataFactory", "dialogService", "constants", "broadcastService", function ($scope, plansListFactory, dataFactory, dialogService, constants, broadcastService) {
			$scope.toggle = function(scope) { scope.toggle(); };
			$scope.ShowModal = function (size, item) {
			    dialogService.showWindow(constants.gkObject.device, item.Device);
			};
			$scope.LoadPlan = function(planId) { dataFactory.getShapesData(planId, function(results) {
				$scope.d3Data = results;
				broadcastService.send('planLoad');
			}); };
			plansListFactory.getPlansList(function(results) {
				if (results.length > 0)
					$scope.PlansList = results;
				else
					$scope.PlansList = { errorMessage: "Планы не загружены. Убедитесь, что планы существуют в текущей конфигурации." };
			});

			//// Получаем данные для отображения плана

			//$scope.d3OnClick = function() { alert("Click"); };
		}
	]);

	app.factory("dataFactory", function($http) {
		return {
			getShapesData: function(planId, callback) { $http.get("../Plans/GetPlan?planGuid=" + planId).success(callback); }
		};
	});

	app.factory("plansListFactory", function($http) {
		return {
			getPlansList: function(callback) { $http.get("../Plans/List").success(callback); }
		};
	});
}());