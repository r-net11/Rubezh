(function () {
    'use strict';

    var app = angular.module('canvasApp.controllers').controller('zonesCtrl',
        function ($scope, $http) {
            $http.get('home/GetFireZonesData').success(function (data, status, headers, config) {
                $scope.data = [];

                $scope.data.push({
                    zone: data.DescriptorPresentationName,
                    id: 1
                });

                $scope.config = {
                    datatype: "local",
                    height: "auto",
                    colNames: ['Id', 'Зона'],
                    colModel:
                        [{ name: 'id', index: 'id', width: 20, sortable: false, hidden: true },
                        { name: 'zone', index: 'zone', width: 250, sortable: false }]
                }
            });

        }
);


}());