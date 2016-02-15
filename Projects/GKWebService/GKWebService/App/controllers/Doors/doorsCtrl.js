(function () {
    var app = angular.module('gkApp.controllers');
    app.controller('doorsCtrl', ['$scope', '$http', '$uibModal', 'signalrDoorsService', 'broadcastService',
        function ($scope, $http, $uibModal, signalrDoorsService, broadcastService) {
            $scope.uiGrid = {
                enableRowSelection: true,
                enableRowHeaderSelection: false,
                multiSelect: false,
                modifierKeysToMultiSelect: true,
                noUnselect: true,
                enableSorting: false,
                enableColumnResizing: true,
                enableColumnMenus: false,
                onRegisterApi: function (gridApi) {
                    $scope.gridApi = gridApi;
                    gridApi.selection.on.rowSelectionChanged($scope, $scope.showSelectedRow);
                },

                columnDefs: [
                    { field: 'No', enableColumnResizing: false, displayName: '№', width: 50, cellTemplate: '<div class="ui-grid-cell-contents"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/{{row.entity.ImageSource}}" />{{row.entity[col.field]}}</div>' },
                    { field: 'Name', width: 200, displayName: 'Наименование', cellTemplate: '<div class="ui-grid-cell-contents"><a href="#" ng-click="grid.appScope.doorClick(row.entity)"><img style="vertical-align: middle; padding-right: 3px" ng-src="/Content/Image/Icon/GKStateIcons/{{row.entity.StateIcon}}.png"/>{{row.entity[col.field]}}</a></div>' },
                    { field: 'DoorType', width: Math.round(($(window).width() - 525) / 2), displayName: 'Тип точки доступа' },
                    { field: 'Desription', width: Math.round(($(window).width() - 525) / 2), displayName: 'Примечание' }
                ]
            };

            $scope.$on('doorChanged', function (event, args) {
                for (var i in $scope.uiGrid.data) {
                    if (args.UID === $scope.uiGrid.data[i].UID) {
                        $scope.uiGrid.data[i] = args;
                        $scope.$apply();
                        break;
                    }
                }
            });

            $scope.doorClick = function (door) {
                var modalInstance = $uibModal.open({
                    animation: false,
                    size: 'rbzh',
                    templateUrl: 'Doors/DoorDetails',
                    controller: 'doorDetailsCtrl',
                    resolve: {
                        door: function () {
                            return door;
                        }
                    }
                });
            };

            $http.get('Doors/GetDoors').success(function (data) {
                $scope.uiGrid.data = data;
            });
        }]);
}());