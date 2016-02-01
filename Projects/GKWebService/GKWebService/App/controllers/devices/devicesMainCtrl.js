(function () {
    'use strict';

    var app = angular.module('gkApp.controllers').controller('devicesMainCtrl',
        function ($scope, $http) {

            $scope.main = function () {
                $http.get('home/GetDevicesList').success(function (data, status, headers, config) {

                    $scope.data = data;

                    for (var i in $scope.data) {
                        var image1 = "<img src= data:image/gif;base64," + $scope.data[i].StateImageSource.Item1 + "> ";
                        var image2 = "<img src= data:image/gif;base64," + $scope.data[i].ImageBloom.Item1 + " height=16 width =16>";

                        $scope.data[i].id = $scope.data[i].UID;
                        $scope.data[i].Expanded = true;
                        $scope.data[i].Name = image1 + image2 + $scope.data[i].Name;

                    }

                    console.debug($scope.data);

                    //$scope.data = [
                    //    {
                    //        id: "1-2",
                    //        Name: "Elem1",
                    //        Address: "Address1",
                    //        UID: 1,
                    //        level: 0,
                    //        parent: null,
                    //        isLeaf: false,
                    //        expanded: true
                    //    },
                    //     {
                    //         id: '2-2',
                    //         Name: "Elem2",
                    //         Address: "Address2",
                    //         UID: 2,
                    //         level: 1,
                    //         parent: '1-2',
                    //         isLeaf: true,
                    //         expanded: false
                    //     },
                    //];


                    $scope.config = {
                        treeGrid: true,
                        treeGridModel: 'adjacency',
                        treeReader: {
                            level_field: "Level",
                            parent_id_field: "ParentUID", // then why does your table use "parent_id"?
                            leaf_field: "IsLeaf",
                            expanded_field: "Expanded"
                        },
                        ExpandColumn: 'Name',
                        datatype: "local",
                        height: "auto",
                        colNames: ['Id', 'Устройство', 'Адрес', 'Примечание'],
                        colModel:
                            [{ name: 'id', index: 'id', sortable: false, hidden: true },
                            { name: 'Name', index: 'Name', width: 250, sortable: false },
                            { name: 'Address', index: 'Address', width: 100, sortable: false },
                            { name: 'Note', index: 'Note', width: $(window).width() - 650, sortable: false }],
                        sortname: 'Name',
                        treeIcons: { plus: 'ui-icon-plusthick', minus: 'ui-icon-minusthick', leaf: 'ui-icon-blank' }
                    }
                });
            };

            $scope.main();
        });
}());