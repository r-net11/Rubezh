(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('organisationsFilterCtrl',
        ['$scope', '$http', '$timeout', '$window',
        function ($scope, $http, $timeout, $window) {
            $scope.gridOptions = {
                onRegisterApi: function (gridApi) { $scope.gridApi = gridApi; },
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableRowSelection: true,
                noUnselect: true,
                columnDefs: [
                    { field: 'Name', width: 210, displayName: 'ФИО', cellTemplate: "<div class='ui-grid-cell-contents'><input style='vertical-align: middle;' type='checkbox' value='' ng-click='grid.appScope.orgClick()' ng-model='row.entity.IsChecked'><img style='vertical-align: middle; padding-right: 3px' ng-src='/Content/Image/Icon/Hr/Organisation.png'/><span ng-style='row.entity.IsDeleted && {opacity:0.5}'>{{row.entity[col.field]}}</span></div>" },
                    { field: 'Description', width: 210, displayName: 'Примечание' }
                ]
            };

            $scope.gridStyle = function () {
                var ctrlHeight = ($window.innerHeight - 100) / 2;
                return "height:" + ctrlHeight + "px";
            }();

            $scope.reloadTree = function() {
                $http.get('Hr/GetOrganisationsFilter', { params: { isWithDeleted: $scope.isWithDeleted } })
                    .then(function(response) {
                        var organisations = response.data.rows;
                        angular.forEach(organisations, function(org) {
                            org.IsChecked = ($scope.filter.OrganisationUIDs.indexOf(org.UID) !== -1);
                        });
                        $scope.gridOptions.data = organisations;
                    });
            };

            $scope.orgClick = function () {
                var orgUIDs = [];
                angular.forEach($scope.gridOptions.data, function (org) {
                    if (org.IsChecked) {
                        orgUIDs.push(org.UID);
                    }
                });
                $scope.filter.OrganisationUIDs = orgUIDs;
            };

            $scope.reloadTree();

            $scope.$watch('isWithDeleted', function() {
                $scope.reloadTree();
            });
        }]
    );

}());
