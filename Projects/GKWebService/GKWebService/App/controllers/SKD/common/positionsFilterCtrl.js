(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('positionsFilterCtrl',
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
                    { field: 'Name', width: 210, displayName: 'Название', cellTemplate: "<div class='ui-grid-cell-contents'><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) ) }\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<input style='vertical-align: middle;' type='checkbox' value='' ng-show='!row.entity.IsOrganisation' ng-click='grid.appScope.posClick()' ng-model='row.entity.IsChecked'><img style='vertical-align: middle; padding-right: 3px' ng-show='!row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Position.png'/><img style='vertical-align: middle; padding-right: 3px' ng-show='row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Organisation.png'/><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" },
                    { field: 'Description', width: 210, displayName: 'Примечание' }
                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = ($window.innerHeight - 100) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            $scope.reloadTree = function() {
                $http.get('Hr/GetPositionsFilter', { params: { isWithDeleted: $scope.isWithDeleted } })
                    .then(function(response) {
                        var positions = response.data.rows;
                        angular.forEach(positions, function (pos) {
                            pos.$$treeLevel = pos.Level;
                            pos.IsChecked = ($scope.filter.PositionUIDs.indexOf(pos.UID) !== -1);
                        });
                        $scope.gridOptions.data = positions;
                        $timeout(function() {
                            $scope.gridApi.treeBase.expandAllRows();
                        });
                    });
            };

            $scope.posClick = function () {
                var posUIDs = [];
                angular.forEach($scope.gridOptions.data, function (pos) {
                    if (pos.IsChecked) {
                        posUIDs.push(pos.UID);
                    }
                });
                $scope.filter.PositionUIDs = posUIDs;
            };

            $scope.reloadTree();

            $scope.toggleRow = function (row, evt) {
                $scope.gridApi.treeBase.toggleRowTreeState(row);
            };

            $scope.$watch('isWithDeleted', function () {
                $scope.reloadTree();
            });
        }]
    );

}());
