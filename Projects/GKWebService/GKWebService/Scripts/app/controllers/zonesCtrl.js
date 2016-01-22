(
    function () {
    'use strict';

    var app = angular.module('canvasApp.controllers').controller('zonesCtrl', ['$scope', '$http','signalrService', function ($scope, $http, signalrService) {
            $scope.signalrService = signalrService;
            $http.get('home/GetFireZonesData').success(function (data, status, headers, config) {
                $scope.data = [];
                var zoneImage = "<img src= data:image/gif;base64," + data.ImageSource.Item1 + "> ";
                var statusImage = "<img src= data:image/gif;base64," + data.StateImageSource.Item1 + "> ";
                $scope.data.push({
                    name: zoneImage + statusImage + data.DescriptorPresentationName,
                    count1: data.Fire1Count,
                    count2: data.Fire2Count
                });

                $scope.config = {
                    datatype: "local",
                    height: "auto",
                    colNames: ['Id', 'Наименование', 'Количество датчиков для перевода в Пожар1:', 'Количество датчиков для перевода в Пожар2:'],
                    colModel:
                        [{ name: 'id', index: 'id', sortable: false, hidden: true },
                        { name: 'name', index: 'name', width: 200, sorztable: false },
                        { name: 'count1', index: 'count1', width: Math.round(($(window).width() - 500) / 2), sortable: false },
                        { name: 'count2', index: 'count2', width: Math.round(($(window).width() - 500) / 2), sortable: false }]
                }});
        }]);
    }()
);