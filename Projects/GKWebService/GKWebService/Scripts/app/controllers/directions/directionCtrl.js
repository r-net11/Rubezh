(function () {

    function imageFormat(cellvalue, options, rowObject) {
        return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/' + rowObject.StateIcon + '.png" />' + rowObject.No;
    };

    function linkFormat(cellvalue, options, rowObject) {
        return "<a href='#' ng-click='config.myClick(\"" + options.rowId + "\")'>" + rowObject.Name + "</a>";
    };

    'use strict';

    var app = angular.module('canvasApp.controllers').controller('directionsCtrl',
        function ($scope, $http, $uibModal) {
            $http.get('Home/GetDirections').success(function (data, status, headers, config) {
                $scope.config = {
                    myClick: function (rowid) {
                        var modalInstance = $uibModal.open({
                            animation: false,
                            templateUrl: 'myModalContent.html',
                            controller: 'ModalInstanceCtrl',
                            resolve: {
                                directionId: function () {
                                    return rowid;
                                }
                            }
                        });
                    },
                    datatype: "local",
                    colModel: [
                        { label: 'UID', name: 'UID', key: true, hidden: true, sortable: false },
                        { label: 'Номер', name: 'No', key: false, hidden: false, sortable: false, formatter: imageFormat },
                        { label: 'Наименование', name: 'Name', hidden: false, sortable: false, formatter: linkFormat },
                        { label: 'Состояние', name: 'State', hidden: false, sortable: false }
                    ],
                    width: jQuery(window).width() - 300,
                    height: jQuery(window).height() - 10,
                    rowNum: 100,
                    viewrecords: true
                };

                $scope.data = data.rows;
            });
        }
    );
    angular.module('canvasApp.controllers').controller('ModalInstanceCtrl', function ($scope, $uibModalInstance, directionId) {

        $scope.directionId = directionId;

        $scope.ok = function () {
            $uibModalInstance.close();
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    });

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
