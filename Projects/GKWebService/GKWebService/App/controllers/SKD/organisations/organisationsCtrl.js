﻿(function () {

    'use strict';

    var app = angular.module('gkApp.controllers').controller('organisationsCtrl',
        ['$scope', '$timeout', 'authService', 'organisationsService',
         function ($scope, $timeout, authService, organisationsService) {
            var reload = function() {
                 organisationsService.getOrganisations($scope.filter)
                     .then(function(organisations) {
                         $scope.organisations = organisations;
                         $scope.model.selectedOrganisation = null;
                     });
            };

             $scope.model = {
                seelctedOrganisation: null
             };

            organisationsService.reload = reload;

            $scope.canSelectUsers = authService.checkPermission('Oper_SKD_Organisations_Users');
            $scope.canSelectDoors = authService.checkPermission('Oper_SKD_Organisations_Doors');

            $scope.$watch('filter', function (newValue, oldValue) {
                reload();
            }, true);

            $scope.$watch('model.selectedOrganisation', function (newValue, oldValue) {
                organisationsService.selectedOrganisation = newValue;
                if (newValue) {
                    organisationsService.getUsers(newValue).then(function (users) {
                        $scope.model.users = users;
                    });
                    organisationsService.getDoors(newValue).then(function (doors) {
                        $scope.model.doors = doors;
                    });
                } else {
                    $scope.model.users = [];
                    $scope.model.doors = [];
                }
            });

            $scope.userClick = function (user) {
                $scope.selectedUser = user;
                organisationsService.setUsersChecked($scope.model.selectedOrganisation, user).then(function (organisation) {
                     $scope.model.selectedOrganisation.UserUIDs = organisation.UserUIDs;
                 });
             };

             $scope.doorClick = function (door) {
                 $scope.selectedDoor = door;
                 organisationsService.setDoorsChecked($scope.model.selectedOrganisation, door).then(function (organisation) {
                     $scope.model.selectedOrganisation.DoorUIDs = organisation.DoorUIDs;
                 });
             };
         }]
    );

}());
