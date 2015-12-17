(function () {
    'use strict';

    var app = angular.module('canvasApp.controllers').controller('firezonesCtrl',
        function ($scope, $http) {

            $http.get('home/GetFireZonesData').success(function (data, status, headers, config) {
                $scope.result = JSON.parse(data);
                var dataSource = $scope.result;
                $scope.data = [{ device: dataSource.Fire1Count, address: dataSource.Fire2Count, logic: dataSource.UID, note: "" }];
            });


            $scope.config = {
                datatype: "local",
                height: 150,
                colNames: ['Устройство', 'Адрес', 'Логика', 'Примечание'],
                colModel:
                    [{ name: 'device', index: 'device', width: 250, sortable: false },
                    { name: 'address', index: 'address', width: 100, sortable: false },
                    { name: 'logic', index: 'logic', width: 50, sortable: false },
                    { name: 'note', index: 'note', width: 780, sortable: false }]
            };

            $scope.data = {};

           


        }
    );

           
}());