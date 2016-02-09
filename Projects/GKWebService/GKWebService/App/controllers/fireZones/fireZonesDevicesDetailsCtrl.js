(function () {

    angular.module('gkApp.controllers').controller('fireZonesDevicesDetailsCtrl',
        function ($scope, uiGridConstants, $uibModalInstance, device) {
            $scope.device = device;

            $scope.$on('devicesChanged', function (event, args) {
                if (args.UID === $scope.device.UID) {
                    $scope.device = args;
                    $scope.$apply();
                };
            });

            var template = '<div align="center" style="color: black; font-weight: bold">{{row.entity[col.field]}}</div>';
            $scope.gridOptions = {
                data: [],
                enableRowHeaderSelection: false,
                enableSorting: false,
                multiSelect: false,
                enableColumnMenus: false,
                enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
                enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
                rowHeight: 35,
                columnDefs: [{ field: 'Name', displayName: 'Параметр', cellTemplate: template }, { field: 'Value', displayName: 'Значение', cellTemplate: template }]
            };

            for (var i in device.Properties) {
                if (device.Properties[i].DriverProperty.IsAUParameter) {
                    var name = device.Properties[i].DriverProperty.Caption;
                    var value = device.Properties[i].Value;
                    if (device.Properties[i].DriverProperty.Parameters.length === 0) {
                        value = device.Properties[i].DriverProperty.Multiplier > 0 ? value / device.Properties[i].DriverProperty.Multiplier : value;
                    }
                    else {
                        for (var j in device.Properties[i].DriverProperty.Parameters) {
                            if (device.Properties[i].DriverProperty.Parameters[j].Value === device.Properties[i].Value) {
                                value = device.Properties[i].DriverProperty.Parameters[j].Name;
                            }
                        }
                    }
                    $scope.gridOptions.data[i] = { Name: name, Value: value };
                }
            };

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