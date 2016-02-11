(function ()
{
    'use strict';

    var app = angular.module('gkApp.controllers').controller('PlanCtrl', [
        '$scope', 'plansListFactory','dataFactory', '$modal', function ($scope, plansListFactory, dataFactory, $modal)
        {
            // Задаем модальное окно для контекстного меню фигуры
            $scope.modal = {
                instance: null
            };

            $scope.ShowModal = function (size, html)
            {
                $scope.modal.instance = $modal.open({
                    template: html,
                    scope: $scope
                });
            };
            $scope.LoadPlan = function(planId) {
                dataFactory.getShapesData(planId, function (results)
                {
                    $scope.d3Data = results;
                });
            };
            plansListFactory.getPlansList(function(results) {
                 $scope.PlansList = results;
            });

            //// Получаем данные для отображения плана
            
            //$scope.d3OnClick = function() { alert("Click"); };
        }
    ]);

    app.factory('dataFactory', function ($http)
    {
        return {
            getShapesData: function (planId, callback) { $http.get('../Plans/GetPlan?planGuid=' + planId).success(callback); }
        };
    });

    app.factory('plansListFactory', function ($http)
    {
        return {
            getPlansList: function (callback) { $http.get('../Plans/GetPlans').success(callback); }
        };
    });
}());