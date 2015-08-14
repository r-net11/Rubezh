(function ()
{
    'use strict';

    var app = angular.module('canvasApp.controllers', ['ui.bootstrap']).controller('PlanCtrl', [
        '$scope', 'dataFactory', '$modal', function ($scope, dataFactory, $modal)
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

            // Получаем данные для отображения плана
            dataFactory.getShapesData(function (results)
            {
                $scope.message = "Request for shapes recieved!";
                $scope.d3Data = results;
            });
            //$scope.d3OnClick = function() { alert("Click"); };
        }
    ]);

    app.factory('dataFactory', function ($http)
    {
        return {
            getShapesData: function (callback) { $http.get('../api/Shapes').success(callback); }
        };
    });
}());