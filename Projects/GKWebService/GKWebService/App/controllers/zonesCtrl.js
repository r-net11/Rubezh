(
    function () {
        'use strict';

        var app = angular.module('gkApp.controllers').controller('zonesCtrl', ['$scope', '$http', 'signalrFireZonesService', function ($scope, $http, signalrFireZonesService) {
            $scope.signalrFireZonesService = signalrFireZonesService;

            $http.get('home/GetFireZonesData').success(function (data, status, headers, config) {
                $scope.data = [];
                for (var i in data) {
                    var item = data[i];
                    var zoneImage = "<img src= data:image/gif;base64," + item.ImageSource.Item1 + "> ";
                    var statusImage = "<img src= data:image/gif;base64," + item.StateImageSource.Item1 + "> ";

                    $scope.data.push({
                        id: i,
                        name: zoneImage + statusImage + item.DescriptorPresentationName,
                        count1: item.Fire1Count,
                        count2: item.Fire2Count
                    });
                }

                $scope.config = {
                    datatype: "local",
                    height: "auto",
                    colNames: ['Id', 'Наименование', 'Количество датчиков для перевода в Пожар1:', 'Количество датчиков для перевода в Пожар2:'],
                    colModel:
                        [{ name: 'id', index: 'id', sortable: false, hidden: true },
                        { name: 'name', index: 'name', width: 200, sorztable: false },
                        { name: 'count1', index: 'count1', width: Math.round(($(window).width() - 500) / 2), sortable: false },
                        { name: 'count2', index: 'count2', width: Math.round(($(window).width() - 500) / 2), sortable: false }]
                }
            });
        }]);
    }()
);