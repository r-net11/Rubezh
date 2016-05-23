(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('positionsCtrl',
        ['$scope', '$timeout', '$window', 'positionsService', 
         function ($scope, $timeout, $window, positionsService) {
            $scope.gridOptions = {
                onRegisterApi: function(gridApi) {
                    $scope.gridApi = gridApi;
                    gridApi.selection.on.rowSelectionChanged($scope, function(row) {
                        positionsService.selectedPosition = row.entity;
                        positionsService.selectedEmployee = null;
                    });
                },                
                enableRowHeaderSelection: false,
                enableSorting: false,
                showTreeExpandNoChildren: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableRowSelection: true,
                noUnselect: true,
                showTreeRowHeader: false,
                columnDefs: [
                    { field: 'Name', width: 210, displayName: 'Название', cellTemplate: "<div class='ui-grid-cell-contents'><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) ) }\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<img style='vertical-align: middle; padding-right: 3px' ng-show='row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Organisation.png'/><img style='vertical-align: middle; padding-right: 3px' ng-show='!row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Position.png'/><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" },
                    { field: 'Description', width: 210, displayName: 'Примечание', cellTemplate: "<div class='ui-grid-cell-contents'><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" }
                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = ($window.innerHeight - 100) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            var reloadTree = function () {
                positionsService.getPositions($scope.filter)
                    .then(function (positions) {
                        $scope.positions = positions;
                        $scope.gridOptions.data = $scope.positions;
                        positionsService.positions = $scope.positions;
                        positionsService.selectedPosiotion = null;
                        positionsService.selectedEmployee = null;
                        $timeout(function() {
                            $scope.gridApi.treeBase.expandAllRows();
                    });
                });
            }

            positionsService.reload = reloadTree;

            $scope.$watch('filter', function (newValue, oldValue) {
                reloadTree();
            }, true);

            $scope.toggleRow = function (row, evt) {
                $scope.gridApi.treeBase.toggleRowTreeState(row);
            };

            $scope.$on('EditOrganisationEvent', function (event, organisation) {
                $scope.updateOrganisation($scope.positions, organisation);
            });

            $scope.$on('AddOrganisationEvent', function (event, organisation) {
                $scope.addOrganisation($scope.gridApi, $scope.positions, organisation);
            });

            $scope.$on('RemoveOrganisationEvent', function (event, organisation) {
                $scope.removeOrganisation($scope.positions, organisation);
            });

            $scope.$on('EditPositionEvent', function (event, position) {
                var oldPosition;
                for (var i = 0; i < $scope.positions.length; i++) {
                    if ($scope.positions[i].UID === position.UID) {
                        oldPosition = $scope.positions[i];
                        break;
                    }
                }

                if (oldPosition) {
                    oldPosition.Description = position.Description;
                    oldPosition.Name = position.Name;
                    oldPosition.RemovalDate = position.RemovalDate;
                    oldPosition.IsDeleted = position.IsDeleted;
                    oldPosition.OrganisationUID = position.OrganisationUID;
                    oldPosition.IsOrganisation = false;
                }
            });

            $scope.$on('AddPositionEvent', function (event, position) {
                for (var i = 0; i < $scope.positions.length; i++) {
                    if ($scope.positions[i].UID === position.OrganisationUID) {
                        var organisation = $scope.positions[i];
                        var newPosition = {
                            UID: position.UID,
                            ParentUID: position.OrganisationUID,
                            Description: position.Description,
                            Name: position.Name,
                            RemovalDate: position.RemovalDate,
                            IsDeleted: position.IsDeleted,
                            OrganisationUID: position.OrganisationUID,
                            IsOrganisation: false,
                            $$treeLevel: 1
                        };
                        $scope.positions.splice(i + 1, 0, newPosition);
                        $timeout(function () {
                            $scope.gridApi.selection.selectRow(newPosition);
                            $scope.gridApi.core.scrollTo(newPosition, $scope.gridOptions.columnDefs[0]);
                        });
                        break;
                    };
                }
            });
         }]
    );

}());
