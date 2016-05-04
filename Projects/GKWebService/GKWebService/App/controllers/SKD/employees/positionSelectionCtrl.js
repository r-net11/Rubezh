(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('positionSelectionCtrl',
        ['$scope', '$http', '$uibModal', '$uibModalInstance', 'organisationUID', 'positionUID', '$timeout', 'positionsService',
         function ($scope, $http, $uibModal, $uibModalInstance, organisationUID, positionUID, $timeout, positionsService) {
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

             $scope.add = function () {
                 var modalInstance = $uibModal.open({
                     animation: false,
                     templateUrl: 'Positions/PositionDetails',
                     controller: 'positionDetailsCtrl',
                     backdrop: 'static',
                     resolve: {
                         position: function () {
                             return positionsService.getPositionDetails(organisationUID, null);
                         },
                         isNew: function () {
                             return true;
                         }
                     }
                 });

                 modalInstance.result.then(function (position) {
                     var newPosition = {
                         UID: position.UID,
                         Name: position.Name,
                         Description: position.Description
                     };
                     $scope.positions.push(newPosition);
                     $timeout(function() {
                         $scope.gridApi.selection.selectRow(newPosition);
                         $scope.gridApi.core.scrollTo(newPosition, $scope.gridOptions.columnDefs[0]);
                     });
                     $scope.$parent.$broadcast('AddPositionEvent', position);
                 });
             };

             $scope.cancel = function () {
                 $uibModalInstance.dismiss('cancel');
             };
         }]
    );

}());
