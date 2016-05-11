(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('departmentSelectionCtrl',
        ['$scope', '$http', '$uibModal', '$q', '$uibModalInstance', 'organisationUID', 'departmentUID', '$timeout', 'departmentsService',
         function ($scope, $http, $uibModal, $q, $uibModalInstance, organisationUID, departmentUID, $timeout, departmentsService) {
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
                 var deferred = $q.defer();

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
                             deferred.resolve(response.data.rows);
                         });
                     });

                 return deferred.promise;
             }

             if (departmentUID) {
                 $scope.title = "Выбор родительского подразделения";
             } else {
                 $scope.title = "Выбор подразделения";
             }

             reloadTree().then();

             $scope.save = function () {
                 $uibModalInstance.close($scope.selectedDepartment);
             };

             $scope.add = function() {
                 var modalInstance = $uibModal.open({
                     animation: false,
                     templateUrl: 'Departments/DepartmentDetails',
                     controller: 'departmentDetailsCtrl',
                     backdrop: 'static',
                     resolve: {
                         department: function () {
                             return departmentsService.getDepartmentDetails(organisationUID, null, $scope.selectedDepartment ? $scope.selectedDepartment.ParentUID : organisationUID);
                         },
                         isNew: function () {
                             return true;
                         },
                         departments: function() {
                             return $scope.departments;
                         }
                     }
                 });

                 modalInstance.result.then(function (department) {
                     reloadTree().then(function() {
                         for (var i = 0; i < $scope.departments.length; i++) {
                             if ($scope.departments[i].UID === department.UID) {
                                 $scope.gridApi.selection.selectRow($scope.departments[i]);
                                 $scope.gridApi.core.scrollTo($scope.departments[i], $scope.gridOptions.columnDefs[0]);
                                 break;
                             }
                         }
                     });
                     $scope.$parent.$broadcast('AddDepartmentEvent', department);
                 });
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
