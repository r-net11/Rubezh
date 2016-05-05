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

        var _markDeleted = function (employee) {
            var deferred = $q.defer();

            $http.post("Employees/MarkDeleted", {
                uid: employee.UID,
                name: employee.Name,
                isOrganisation: employee.IsOrganisation
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка сохранения сотрудника");
                deferred.reject();
            });

            return deferred.promise;
        };

        var _getSchedules = function (organisationUID) {
            var deferred = $q.defer();

            $http.get('Employees/GetSchedules', { params: { organisationUID: organisationUID } })
                .then(function (response) {
                    deferred.resolve(response.data.Schedules);
                }, function (response) {
                    // TODO: реализовать обработку ошибок
                    alert("Ошибка получения графиков работы");
                    deferred.reject();
            });

            return deferred.promise;
        };

        var _getEmployeeCardDetails = function (UID, organisationUID) {
            var deferred = $q.defer();

            $http.get('Employees/GetEmployeeCardDetails', { params: { organisationId: organisationUID, cardId: UID } })
                .then(function (response) {
                    deferred.resolve(response.data);
                }, function (response) {
                    // TODO: реализовать обработку ошибок
                    alert("Ошибка получения пропуска");
                    deferred.reject();
            });

            return deferred.promise;
        };

        var _getOrganisationDepartmentEmployees = function (organisationUID, departmentUID) {
            var deferred = $q.defer();

            $http.get('Hr/GetOrganisationDepartmentEmployees', { params: { organisationId: organisationUID, departmentId: departmentUID } })
                .then(function (response) {
                    deferred.resolve(response.data.Employees);
                }, function (response) {
                    // TODO: реализовать обработку ошибок
                    alert("Ошибка получения сотрудников");
                    deferred.reject();
            });

            return deferred.promise;
        };

        var _saveEmployeeCardDetails = function (card, employee, organisationUID, isNew) {
            var deferred = $q.defer();

            card.Card.OrganisationUID = organisationUID;
            card.Card.EmployeeUID = employee.UID;
            card.Card.EmployeeName = employee.Name;

            $http.post("Employees/EmployeeCardDetails", {
                cardModel: card,
                employeeName: employee.Name,
                isNew: isNew
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка сохранения сотрудника");
                deferred.reject();
            });

            return deferred.promise;
        };

        var _deleteCard = function (UID) {
            var deferred = $q.defer();

            $http.post("Employees/DeleteCard", {
                id: UID,
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка сохранения сотрудника");
                deferred.reject();
            });

            return deferred.promise;
        };

        var _deleteCardFromEmployee = function (UID, employeeName, removalReason) {
            var deferred = $q.defer();

            $http.post("Employees/DeleteFromEmployee", {
                id: UID,
                employeeName: employeeName,
                reason: removalReason
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                // TODO: реализовать обработку ошибок
                alert("Ошибка сохранения сотрудника");
                deferred.reject();
            });

            return deferred.promise;
        };

        return {
            selectedEmployee: null,
            selectedCard: null,
            reload: null,
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
            getOrganisations: function(filter) {
                var deferred = $q.defer();

                $http.get('Employees/GetOrganisations', { params: filter }).then(function (response) {
                    deferred.resolve(response.data.rows);
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
            getSingleShort: function (UID) {
                var deferred = $q.defer();

                $http.get('Employees/GetSingleShort/' + UID).then(function (response) {
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
            },
            markDeleted: _markDeleted,
            getEmployeeCardDetails: _getEmployeeCardDetails,
            saveEmployeeCardDetails: _saveEmployeeCardDetails,
            deleteCard: _deleteCard,
            getSchedules: _getSchedules,
            deleteCardFromEmployee: _deleteCardFromEmployee,
            getOrganisationDepartmentEmployees: _getOrganisationDepartmentEmployees
        }
    }]);
}());
