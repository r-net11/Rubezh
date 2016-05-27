(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('accessTemplatesCtrl',
        ['$scope', '$timeout', '$window', 'accessTemplatesService',
         function ($scope, $timeout, $window, accessTemplatesService) {
             $scope.gridOptionsAccessTemplates = {
                onRegisterApi: function(gridApi) {
                    $scope.gridApi = gridApi;
                    gridApi.selection.on.rowSelectionChanged($scope, function(row) {
                        accessTemplatesService.selectedAccessTemplate = row.entity;
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
                    { field: 'Name', width: 210, displayName: 'Название', cellTemplate: "<div class='ui-grid-cell-contents'><div style=\"float:left;\" class=\"ui-grid-tree-base-row-header-buttons\" ng-class=\"{'ui-grid-tree-base-header': row.treeLevel > -1 }\" ng-click=\"grid.appScope.toggleRow(row,evt)\"><i ng-class=\"{'ui-grid-icon-minus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'expanded', 'ui-grid-icon-plus-squared': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length > 0 ) ) && row.treeNode.state === 'collapsed', 'ui-grid-icon-blank': ( ( grid.options.showTreeExpandNoChildren && row.treeLevel > -1 ) || ( row.treeNode.children && row.treeNode.children.length == 0 ) )}\" ng-style=\"{'padding-left': grid.options.treeIndent * row.treeLevel + 'px'}\"></i> &nbsp;</div>{{ CUSTOM_FILTERS}}<img style='vertical-align: middle; padding-right: 3px' ng-show='row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/Organisation.png'/><img style='vertical-align: middle; padding-right: 3px' ng-show='!row.entity.IsOrganisation' ng-src='/Content/Image/Icon/Hr/AccessTemplate.png'/><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" },
                    { field: 'Description', width: 210, displayName: 'Примечание', cellTemplate: "<div class='ui-grid-cell-contents'><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" }
                ]
            };

             $scope.gridOptionsDoors = {
                 enableRowHeaderSelection: false,
                 enableSorting: false,
                 multiSelect: false,
                 enableColumnMenus: false,
                 enableRowSelection: true,
                 noUnselect: true,
                 columnDefs: [
                     { field: 'PresentationName', width: 210, displayName: 'Точка доступа' },
                     { field: 'EnerScheduleName', width: 210, displayName: 'Вход' },
                     { field: 'ExitScheduleName', width: 210, displayName: 'Выход' }
                 ]
             };

             $scope.gridStyle = function () {
                var ctrlHeight = ($window.innerHeight - 100) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            var reloadTree = function () {
                accessTemplatesService.getAccessTemplates($scope.filter)
                    .then(function (accessTemplate) {
                        $scope.accessTemplates = accessTemplate;
                        $scope.gridOptionsAccessTemplates.data = $scope.accessTemplates;
                        accessTemplatesService.accessTemplates = $scope.accessTemplates;
                        accessTemplatesService.selectedAccessTemplate = null;
                        $timeout(function() {
                            $scope.gridApi.treeBase.expandAllRows();
                    });
                });
            }

            accessTemplatesService.reload = reloadTree;

            $scope.$watch('filter', function (newValue, oldValue) {
                reloadTree();
            }, true);

            $scope.toggleRow = function (row, evt) {
                $scope.gridApi.treeBase.toggleRowTreeState(row);
            };

            var reloadDoors = function () {
                if ($scope.selectedAccessTemplate && !$scope.selectedAccessTemplate.IsOrganisation) {
                    accessTemplatesService.getDoors($scope.selectedAccessTemplate.UID)
                        .then(function(doors) {
                            $scope.doors = doors;
                            $scope.gridOptionsDoors.data = $scope.doors;
                        });
                } else {
                    $scope.doors = [];
                    $scope.gridOptionsDoors.data = [];
                }
            }

            $scope.$watch(function () {
                return accessTemplatesService.selectedAccessTemplate;
            }, function (accessTemplate) {
                $scope.selectedAccessTemplate = accessTemplate;
                reloadDoors();
            });

            $scope.$on('EditOrganisationEvent', function (event, organisation) {
                $scope.updateOrganisation($scope.accessTemplates, organisation);
            });

            $scope.$on('AddOrganisationEvent', function (event, organisation) {
                $scope.addOrganisation($scope.gridApi, $scope.accessTemplates, organisation);
            });

            $scope.$on('RemoveOrganisationEvent', function (event, organisation) {
                $scope.removeOrganisation($scope.accessTemplates, organisation);
            });

            $scope.$on('EditAccessTemplateEvent', function (event, accessTemplate) {
                var oldAccessTemplate;
                for (var i = 0; i < $scope.accessTemplates.length; i++) {
                    if ($scope.accessTemplates[i].UID === accessTemplate.UID) {
                        oldAccessTemplate = $scope.accessTemplates[i];
                        break;
                    }
                }

                if (oldAccessTemplate) {
                    oldAccessTemplate.Description = accessTemplate.Description;
                    oldAccessTemplate.Name = accessTemplate.Name;
                    oldAccessTemplate.RemovalDate = accessTemplate.RemovalDate;
                    oldAccessTemplate.IsDeleted = accessTemplate.IsDeleted;
                    oldAccessTemplate.OrganisationUID = accessTemplate.OrganisationUID;
                    oldAccessTemplate.IsOrganisation = false;
                }
            });

            $scope.$on('AddAccessTemplateEvent', function (event, accessTemplate) {
                for (var i = 0; i < $scope.accessTemplates.length; i++) {
                    if ($scope.accessTemplates[i].UID === accessTemplate.OrganisationUID) {
                        var organisation = $scope.accessTemplates[i];
                        var newAccessTemplate = {
                            UID: accessTemplate.UID,
                            ParentUID: accessTemplate.OrganisationUID,
                            Description: accessTemplate.Description,
                            Name: accessTemplate.Name,
                            RemovalDate: accessTemplate.RemovalDate,
                            IsDeleted: accessTemplate.IsDeleted,
                            OrganisationUID: accessTemplate.OrganisationUID,
                            IsOrganisation: false,
                            $$treeLevel: 1
                        };
                        $scope.accessTemplates.splice(i + 1, 0, newAccessTemplate);
                        $timeout(function () {
                            $scope.gridApi.selection.selectRow(newAccessTemplate);
                            $scope.gridApi.core.scrollTo(newAccessTemplate, $scope.gridOptionsAccessTemplates.columnDefs[0]);
                        });
                        break;
                    };
                }
            });

         }]
    );

}());
