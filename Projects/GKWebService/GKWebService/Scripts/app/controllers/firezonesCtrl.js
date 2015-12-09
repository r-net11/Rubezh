(function () {
    'use strict';

    var app = angular.module('canvasApp.controllers').controller('firezonesCtrl',
        function ($scope, $http) {
            $http.get('home/GetFireZonesData').success(function (data, status, headers, config) {
                $scope.data = [];
                for (var i in data.devicesList) {
                            $scope.data.push({
                                device: "<img src= data:image/gif;base64," +
                                                data.devicesList[i].ImageBloom.Item1 +
                                                " height=" + data.devicesList[i].ImageBloom.Item2.Height / 2 +
                                                "width =" + data.devicesList[i].ImageBloom.Item2.Width / 2 + "> " + data.devicesList[i].ShortName,
                                address: data.devicesList[i].Address,
                                logic: "",
                                note: "",
                                id: i,
                                level: i,
                                parent: (i > 0) ? (i - 1).toString() : "null",
                                isLeaf: (i == data.devicesList.length - 1) ? true : false,
                                expanded: true
                            });
                }
                
                $scope.config = {
                    treeGrid: true,
                            treeGridModel: 'adjacency',
                            ExpandColumn: 'device',
                            datatype: "local",
                            height: "auto",
                            colNames: ['Id', 'Устройство', 'Адрес', 'Логика', 'Примечание'],
                            colModel:
                                [{ name: 'id', index: 'id', width: 20, sortable: false, hidden: true },
                                { name: 'device', index: 'device', width: 250, sortable: false },
                                { name: 'address', index: 'address', width: 100, sortable: false },
                                { name: 'logic', index: 'logic', width: 50, sortable: false },
                                { name: 'note', index: 'note', width: 780, sortable: false }],
                            sortname: 'id',
                            treeIcons: { plus: 'ui-icon-plusthick', minus: 'ui-icon-minusthick', leaf: 'ui-icon-blank' }
                        }
            });

        }
);


}());