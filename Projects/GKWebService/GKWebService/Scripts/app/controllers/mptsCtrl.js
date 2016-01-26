(function () {
    'use strict';
        var app = angular.module('canvasApp.controllers').controller('mptsCtrl',
           function ($scope, $http) {
               $scope.uiGrid = {
                   columnDefs:
                     [{ name: 'No', displayName: 'No', width: 50 },
                      { name: 'Name', displayName: 'МПТ', width: 450 },
                      { name: 'Delay', displayName: 'Задержка', width:200}],
               },

               $http.get('home/GetMPTsData').success(function (data) {
                   $scope.uiGrid.data = data;

               });
           });
}());