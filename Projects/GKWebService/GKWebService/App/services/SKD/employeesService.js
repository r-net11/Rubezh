(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('employeesService', ['$http', '$q', function ($http, $q) {
        var saveChief = function (employeeUID, organisation, isOrganisationChief, organisationChiefUID, url) {
            var deferred = $q.defer();
            var uid;
            if (isOrganisationChief && organisationChiefUID !== employeeUID) {
                uid = employeeUID;
            } else if (!isOrganisationChief && organisationChiefUID === employeeUID) {
                uid = "";
            }
            if (angular.isDefined(uid)) {
                $http.post(url, {
                    OrganisationUID: organisation.UID,
                    EmployeeUID: uid,
                    OrganisationName: organisation.Name
                }).then(function() {
                    deferred.resolve();
                });
            } else {
                deferred.resolve();
            }
            return deferred.promise;
        };

        return {
            selectedEmployee: null,
            getEmployeeDetails: function(UID) {
                var deferred = $q.defer();

                $http.get('Employees/GetEmployeeDetails/' + UID).then(function (response) {
                    deferred.resolve(response.data);
                }, function (response) {
                    // TODO: реализовать обработку ошибок
                    alert("Ошибка получения сотрудника");
                    deferred.reject();
                });

                return deferred.promise;
            },
            getOrganisation: function(UID) {
                var deferred = $q.defer();

                $http.get('Employees/GetOrganisation/' + UID).then(function (response) {
                    deferred.resolve(response.data);
                }, function (response) {
                    // TODO: реализовать обработку ошибок
                    alert("Ошибка получения сотрудника");
                    deferred.reject();
                });

                return deferred.promise;
            },
            getEmployeeCards: function (UID) {
                var deferred = $q.defer();

                $http.get('Employees/GetEmployeeCards/' + UID).then(function (response) {
                    deferred.resolve(response.data);
                }, function (response) {
                    // TODO: реализовать обработку ошибок
                    alert("Ошибка получения сотрудника");
                    deferred.reject();
                });

                return deferred.promise;
            },
            getEmployeePhoto: function (UID) {
                var deferred = $q.defer();

                $http.get('Employees/GetEmployeePhoto/' + UID).then(function (response) {
                    deferred.resolve(response.data);
                }, function (response) {
                    // TODO: реализовать обработку ошибок
                    alert("Ошибка получения сотрудника");
                    deferred.reject();
                });

                return deferred.promise;
            },
            saveEmployee: function (employee, photoData, isNew, organisation, isOrganisationChief, isOrganisationHRChief) {
                var deferred = $q.defer();

                if (photoData === "//:0") {
                    photoData = null;
                }

                $http.post("Employees/EmployeeDetails", {
                    employee: { Employee: employee, PhotoData: photoData },
                    isNew: isNew
                }).then(function (response) {
                    return saveChief(employee.UID, organisation, isOrganisationChief, organisation.ChiefUID, "Employees/SaveChief");
                }).then(function (response) {
                    return saveChief(employee.UID, organisation, isOrganisationHRChief, organisation.HRChiefUID, "Employees/SaveHRChief");
                }).then(function (response) {
                    deferred.resolve();
                }).catch(function (response) {
                    // TODO: реализовать обработку ошибок
                    alert("Ошибка сохранения сотрудника");
                    deferred.reject();
                });

                return deferred.promise;
            }
        }
    }]);
}());
