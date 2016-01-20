(function () {
    'use strict';

    var app = angular.module('canvasApp.controllers').controller('zonesCtrl',
        function ($scope, $http) {
            $http.get('home/GetFireZonesData').success(function (data, status, headers, config) {
                $scope.data = [];

                $scope.data.push({
                    name: "<img src= data:image/gif;base64," + data.ImageSource.Item1 + "> " + data.DescriptorPresentationName,
                    status: "<img src= data:image/gif;base64," + data.StateImageSource.Item1 + "> ",
                    count1: data.Fire1Count,
                    count2: data.Fire2Count
            });

                $scope.config = {
                    datatype: "local",
                    height: "auto",
                    colNames: ['Id', 'Наименование', 'Статус', 'Количество датчиков для перевода в Пожар1:', 'Количество датчиков для перевода в Пожар2:'],
                    colModel:
                        [{ name: 'id', index: 'id', sortable: false, hidden: true },
                        { name: 'name', index: 'name', sortable: false },
                        { name: 'status', index: 'status', width: "100px", sortable: false },
                        { name: 'count1', index: 'count1', width: "300px", sortable: false },
                        { name: 'count2', index: 'count2', width: "300px", sortable: false }]
                }
            });

        }
);

}());