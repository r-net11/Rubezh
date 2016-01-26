(function () {
    'use strict';

    var app = angular.module('canvasApp.controllers').controller('devicesCtrl',
        function ($scope, $http) {
            $http.get('home/GetDevicesListByZoneNumber/0').success(function (data, status, headers, config) {
                $scope.data = [];
                for (var i in data) {
                    var item = data[i].DeviceList;
                    for (var j in item) {
                        var element = item[j];
                        var image1 = "<img src= data:image/gif;base64," + element.StateImageSource.Item1 + "> ";
                        var image2 = "<img src= data:image/gif;base64," + element.ImageBloom.Item1 + " height=16 width =16>";

                        $scope.data.push({
                            device: i > 0 ? image1 + image2 + element.Name : image2 + element.Name,
                            address: element.Address,
                            note: element.Note,
                            id: (parseInt(i) + parseInt(j)).toString(),
                            level: data.length - element.Level,
                            parent: (i > 0) ? (i - 1) : null,
                            isLeaf: (i == data.length - 1) ? true : false,
                            expanded: true
                        });
                    }
                    
                }

                $scope.config = {
                    treeGrid: true,
                    treeGridModel: 'adjacency',
                    ExpandColumn: 'device',
                    datatype: "local",
                    height: "auto",
                    colNames: ['Id', 'Устройство', 'Адрес', 'Примечание'],
                    colModel:
                        [{ name: 'id', index: 'id', sortable: false, hidden: true },
                        { name: 'device', index: 'device', width: 250, sortable: false },
                        { name: 'address', index: 'address', width: 100, sortable: false },
                        { name: 'note', index: 'note', width: $(window).width() - 650, sortable: false }],
                    sortname: 'id',
                    treeIcons: { plus: 'ui-icon-plusthick', minus: 'ui-icon-minusthick', leaf: 'ui-icon-blank' }
                }
            });

        }
);


}());