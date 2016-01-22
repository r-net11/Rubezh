(function () {
    'use strict';

    var app = angular.module('canvasApp.controllers').controller('mptsCtrl',
        function ($scope, $http) {
            $http.get('home/GetMPTsData').success(function (data, status, headers, config) {
                $scope.data = [];
                for (var i in data) {
                    $scope.data.push({
                        name: data[i].Name,
                        No: data[i].No
                    });
                }

                $scope.config = {
                    datatype: "local",
                    colNames: ['Id', 'No', 'Наименование'],
                    colModel:
                        [{ name: 'id', index: 'id', sortable: false, hidden: true },
                        { name: 'No', index: 'No', sortable: false, width: "1px" },
                        { name: 'name', index: 'name', width: 5, sortable: false, }],
                    width: jQuery(window).width() - 1000,
                    height: jQuery(window).height() - 10,
                    rowNum: 100,
                    viewrecords: true
                }
            });

        }
);

}());