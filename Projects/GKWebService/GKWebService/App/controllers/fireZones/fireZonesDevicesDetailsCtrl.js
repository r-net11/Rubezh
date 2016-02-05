(function () {

    angular.module('gkApp.controllers').controller('fireZonesDevicesDetailsCtrl',
        function ($scope, $uibModalInstance, device) {
            $scope.device = device;

            $scope.$on('fireZonesDeviceChanged', function (event, args) {
                if (args.Uid === $scope.device.Uid) {
                    $scope.device = args;
                    $scope.$apply();
                };
            });

            $scope.gridOptions = {
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableRowSelection: true,
                columnDefs:
                    [
                        { field: 'Name', displayName: 'Параметр', width: 275, cellTemplate: '<span style="font-weight: bold; color: black">{{row.entity[col.field]}}</span>' },
                        { field: 'Value', displayName: 'Значение', width: 275, cellTemplate: '<span style="font-weight: bold; color: black">{{row.entity[col.field]}}</span>' }
                    ]
            };
            $scope.gridOptions.data = device.Properties;


            $scope.Show = function () {

            };

            $scope.ShowJournal = function () {

            };

            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };
        }
    );
}());