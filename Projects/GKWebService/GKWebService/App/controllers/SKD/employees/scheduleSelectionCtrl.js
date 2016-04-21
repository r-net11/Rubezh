(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('scheduleSelectionCtrl',
        ['$scope', '$http', '$uibModalInstance', 'schedules', 'scheduleStartDate',
         function ($scope, $http, $uibModalInstance, schedules, scheduleStartDate) {
             $scope.popupScheduleStartDate = {
                 opened: false
             };

             $scope.model = {
                 scheduleStartDate: scheduleStartDate,
                 selectedSchedule: null
             };

             $scope.schedules = schedules;

             $scope.save = function () {
                 $uibModalInstance.close($scope.model);
             };

             $scope.cancel = function () {
                 $uibModalInstance.dismiss('cancel');
             };
         }]
    );

}());
