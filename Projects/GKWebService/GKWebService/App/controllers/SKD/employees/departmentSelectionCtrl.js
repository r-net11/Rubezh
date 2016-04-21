(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('departmentSelectionCtrl',
        ['$scope', '$http', '$uibModalInstance', 'organisationUID', 'departmentUID', '$timeout', '$window',
         function ($scope, $http, $uibModalInstance, organisationUID, departmentUID, $timeout, $window) {
             $scope.gridOptions = {
                 onRegisterApi: function (gridApi) {
                     $scope.gridApi = gridApi;
                     gridApi.selection.on.rowSelectionChanged($scope, function (row) {
                         $scope.selectedDepartment = row.entity;
                     });
                 },
                 enableRowHeaderSelection: false,
                 enableSorting: false,
                 showTreeExpandNoChildren: false,
                 multiSelect: false,
                 enableColumnMenus: false,
                 enableRowSelection: true,
                 noUnselect: true,
                 columnDefs: [
                     { field: 'Name', width: 210, displayName: 'Название' }
                 ]
             };

             $scope.gridStyle = function () {
                 return "height: 300px";
             }();

             var reloadTree = function () {
                 $http.get('Employees/GetDepartments', {
                     params: { organisationUID: organisationUID, departmentUID: departmentUID }
                 })
                     .then(function (response) {
                         $scope.departments = response.data.rows;
                         angular.forEach($scope.departments, function (value, key) {
                             value.$$treeLevel = value.Level;
                         });
                         $scope.gridOptions.data = $scope.departments;
                         $scope.selectedDepartment = null;
                         $timeout(function () {
                             $scope.gridApi.treeBase.expandAllRows();
                         });
                     });
             }

             if (departmentUID) {
                 $scope.title = "Выбор родительского подразделения";
             } else {
                 $scope.title = "Выбор подразделения";
             }

             reloadTree();

             $scope.save = function () {
                 $uibModalInstance.close($scope.selectedDepartment);
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
