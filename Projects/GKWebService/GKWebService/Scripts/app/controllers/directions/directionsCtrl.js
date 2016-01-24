(function () {

    function imageFormat(cellvalue, options, rowObject) {
        return '<img style="vertical-align: middle; padding-right: 3px" src="/Content/Image/Icon/GKStateIcons/' + rowObject.StateIcon + '.png" />' + rowObject.No;
    };

    function linkFormat(cellvalue, options, rowObject) {
        return "<a href='#' ng-click='config.myClick(\"" + options.rowId + "\")'>" + rowObject.Name + "</a>";
    };

    'use strict';

    var app = angular.module('canvasApp.controllers').controller('directionsCtrl',
        ['$scope', '$http', '$uibModal', 'signalrDirectionsService',
        function ($scope, $http, $uibModal, signalrDirectionsService) {
            $http.get('Home/GetDirections').success(function (data, status, headers, config) {
                $scope.config = {
                    myClick: function (rowid) {
                        var modalInstance = $uibModal.open({
                            animation: false,
                            templateUrl: 'Directions/DirectionDetails',
                            controller: 'directionDetailsCtrl',
                            resolve: {
                                direction: function () {
                                    var direction = null;
                                    for (var i = 0; i < data.rows.length; i++) {
                                        if (data.rows[i].UID === rowid) {
                                            direction = data.rows[i];
                                        }
                                    }
                                    return direction;
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

                $scope.signalrDirectionsService = signalrDirectionsService;
            });
        }]
    );

}());
