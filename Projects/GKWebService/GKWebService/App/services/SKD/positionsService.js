(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('positionsService', ['$http', '$q', function ($http, $q) {
        var _getPositions = function(filter) {
            var deferred = $q.defer();

            $http.get('Positions/GetOrganisations', { params: filter }).then(function (response) {
                angular.forEach(response.data.rows, function (value, key) {
                    value.$$treeLevel = value.Level;
                });
                deferred.resolve(response.data.rows);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения должности");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getPositionDetails = function (organisationUID, UID) {
            var deferred = $q.defer();

            $http.get('Positions/GetPositionDetails', {
                params: {
                organisationId: organisationUID,
                id: UID
            }
            }).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения должности");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getPositionEmployeeList = function (filter) {
            var deferred = $q.defer();

            $http.get('Positions/GetPositionEmployeeList', {
                 params: {
                     positionId: this.selectedPosition.UID,
                     organisationId: this.selectedPosition.OrganisationUID,
                     isWithDeleted: filter.LogicalDeletationType === "All"
                 }
            }).then(function (response) {
                deferred.resolve(response.data.rows);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения сотрудников должности");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _saveEmployeePosition = function (employee, selectedPositionUID) {
            var deferred = $q.defer();

            $http.post('Positions/SaveEmployeePosition', {
                employeeUID: employee.UID,
                positionUID: selectedPositionUID,
                name: employee.Name
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка сохранения сотрудника должности");
                deferred.reject();
            });

            return deferred.promise;
        };

        var _savePosition = function (position, isNew) {
            var deferred = $q.defer();

            if (position.photoData === "//:0") {
                position.photoData = null;
            }

            $http.post('Positions/PositionDetails', {
                position: position,
                isNew: isNew
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка сохранения должности");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _markDeleted = function (UID, name) {
            var deferred = $q.defer();

            $http.post('Positions/MarkDeleted', {
                uid: UID,
                name: name
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка удаления должности");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getChildEmployeeUIDs = function (UID) {
            var deferred = $q.defer();

            $http.get('Positions/GetChildEmployeeUIDs', { params: { positionId: UID } }).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка получения сотрудников");
                deferred.reject();
            });

            return deferred.promise;
        };

        return {
            selectedPosition: null,
            reload: null,
            getPositions: _getPositions,
            getPositionEmployeeList: _getPositionEmployeeList,
            savePosition: _savePosition,
            saveEmployeePosition: _saveEmployeePosition,
            getPositionDetails: _getPositionDetails,
            markDeleted: _markDeleted,
            getChildEmployeeUIDs: _getChildEmployeeUIDs
        }
    }]);
}());
