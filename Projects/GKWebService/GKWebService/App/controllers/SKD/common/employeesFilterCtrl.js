(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('employeesFilterCtrl',
        ['$scope', '$http', '$timeout', '$window',
        function ($scope, $http, $timeout, $window) {
            $scope.gridOptions = {
                onRegisterApi: function (gridApi) { $scope.gridApi = gridApi; },
                enableRowHeaderSelection: false,
                enableSorting: false,
                showTreeExpandNoChildren: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableRowSelection: true,
                noUnselect: true,
                showTreeRowHeader: false,
                columnDefs: [
                    { field: 'Name', width: 410, displayName: 'ФИО', cellTemplate: "<div class='ui-grid-cell-contents'><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) ) && row.treeNode.state === 'expanded'}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<input style='vertical-align: middle;' type='checkbox' value='' ng-show='!row.entity.IsOrganisation' ng-click='grid.appScope.empClick()' ng-model='row.entity.IsChecked'><img style='vertical-align: middle; padding-right: 3px' ng-show='!row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Employee.png'/><img style='vertical-align: middle; padding-right: 3px' ng-show='row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Organisation.png'/><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" }
                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = ($window.innerHeight - 100) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            if ($scope.filter.LastName || $scope.filter.FirstName || $scope.filter.SecondName) {
                $scope.isSearch = true;
            }

            $scope.reloadTree = function() {
                $http.get('Hr/GetEmployeesFilter', { params: {
                             isWithDeleted: $scope.isWithDeleted,
                             selectedPersonType: $scope.personType
                        }
                    })
                    .then(function(response) {
                        var employees = response.data.rows;
                        angular.forEach(employees, function (emp) {
                            emp.$$treeLevel = emp.Level;
                            emp.IsChecked = ($scope.filter.UIDs.indexOf(emp.UID) !== -1);
                        });
                        $scope.gridOptions.data = employees;
                        $timeout(function() {
                            $scope.gridApi.treeBase.expandAllRows();
                        });
                    });
            };

            $scope.empClick = function () {
                var empUIDs = [];
                angular.forEach($scope.gridOptions.data, function (emp) {
                    if (emp.IsChecked) {
                        empUIDs.push(emp.UID);
                    }
                });
                $scope.filter.UIDs = empUIDs;
            };

            $scope.reloadTree();

            $scope.toggleRow = function (row, evt) {
                $scope.gridApi.treeBase.toggleRowTreeState(row);
            };

            $scope.$watch('isWithDeleted', function () {
                $scope.reloadTree();
            });

            $scope.$watch('isSearch', function (value) {
                if (!value) {
                    $scope.filter.LastName = '';
                    $scope.filter.FirstName = '';
                    $scope.filter.SecondName = '';
                }
            });
        }]
    );

}());
