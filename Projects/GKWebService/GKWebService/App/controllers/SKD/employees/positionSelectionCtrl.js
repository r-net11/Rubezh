(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('positionSelectionCtrl',
        ['$scope', '$http', '$uibModalInstance', 'organisationUID', 'positionUID', '$timeout',
         function ($scope, $http, $uibModalInstance, organisationUID, positionUID, $timeout) {
             $scope.gridOptions = {
                 onRegisterApi: function (gridApi) {
                     $scope.gridApi = gridApi;
                     gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                         $scope.selectedPosition = row.entity;
                     });
                 },
                 enableRowHeaderSelection: false,
                 enableSorting: false,
                 multiSelect: false,
                 enableColumnMenus: false,
                 enableRowSelection: true,
                 noUnselect: true,
                 columnDefs: [
                     { field: 'Name', width: 210, displayName: 'Название' },
                     { field: 'Description', width: 210, displayName: 'Описание' }
                 ]
             };

             $scope.gridStyle = function () {
                 return "height: 300px";
             }();

             var reload = function () {
                 $http.get('Employees/GetPositions', {
                     params: { organisationUID: organisationUID, positionUID: positionUID }
                 })
                     .then(function (response) {
                         $scope.positions = response.data.rows;
                         $scope.gridOptions.data = $scope.positions;
                         $scope.selectedPosition = null;
                     });
             }

             reload();

             $scope.save = function () {
                 $uibModalInstance.close($scope.selectedPosition);
             };

             $scope.clear = function () {
                 $uibModalInstance.close(null);
             };

             $scope.cancel = function () {
                 $uibModalInstance.dismiss('cancel');
             };
         }]
    );

}());
