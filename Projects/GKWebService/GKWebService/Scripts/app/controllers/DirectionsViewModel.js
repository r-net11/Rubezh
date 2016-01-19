(function () {
    'use strict';

    var app = angular.module('canvasApp.controllers').controller('directionsCtrl',
        function ($scope, $http) {
            $scope.config = {
                datatype: "local",
                colModel: [
                    { label: 'No', name: 'No', key: true, hidden: false, sortable: false },
                    { label: 'Name', name: 'Name', width: 100, hidden: false, sortable: false }
                ],
                width: 630,
                height: 500,
                rowNum: 100,
                viewrecords: true
            };

            $scope.data = [];

            $http.get('Home/GetDirections').success(function (data, status, headers, config) {
                $scope.data = data.rows;
            });
        }
);


}());

//$(document).ready(function () {
//    $("#jqGridDirections").jqGrid({
//        url: '/Home/GetDirections',
//        datatype: "json",
//        colModel: [
//            { label: 'No', name: 'No', key: true, hidden: false, sortable: false },
//            { label: 'Name', name: 'Name', hidden: false, sortable: false }
//        ],
//        width: jQuery(window).width() - 242,
//        height: 250,
//        rowNum: 100,
//        viewrecords: true
//    });

//});

//function DirectionsViewModel() {
//    var self = {};

//    self.No = ko.observable();
//    self.Name = ko.observable();

//    $('#jqGridDirections').on('jqGridSelectRow', function (event, id, selected) {

//        var myGrid = $('#jqgrid');

//        self.No(myGrid.jqGrid('getCell', id, 'No'));
//        self.Name(myGrid.jqGrid('getCell', id, 'Name'));

//    });

//    return self;
//}
