(function() {
	"use strict";

	var app = angular.module("gkApp.controllers").controller("PlanCtrl", [
		"$scope", "$timeout", "plansListFactory", "dataFactory", "$stateParams", "dialogService", "constants", "broadcastService", function ($scope, $timeout, plansListFactory, dataFactory, $stateParams, dialogService, constants, broadcastService) {
			$scope.toggle = function(scope) { scope.toggle(); };
			$scope.ShowModal = function (size, item) {
			    if (item.GkObject.ObjectType == constants.gkObject.device.type) {
			        dialogService.showWindow(constants.gkObject.device, item.GkObject);
			    } else if (item.GkObject.ObjectType == constants.gkObject.zone.type) {
			        dialogService.showWindow(constants.gkObject.zone, item.GkObject);
			    } else if (item.GkObject.ObjectType == constants.gkObject.guardZone.type) {
			        dialogService.showWindow(constants.gkObject.guardZone, item.GkObject);
			    }
			};
			$scope.LoadPlan = function(planId) { dataFactory.getShapesData(planId, function(results) {
				$scope.d3Data = results;
				broadcastService.send('planLoad');
			}); };
			plansListFactory.getPlansList(function(results) {
				if (results.length > 0) {
					$scope.PlansList = results;
					$timeout(function () {
						if ($stateParams.uid) {
							$scope.LoadPlan($stateParams.uid);
						}
					});
				}
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