(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('departmentsService', ['$http', '$q', 'dialogService', function ($http, $q, dialogService) {
        var _getDepartments = function(filter) {
            var deferred = $q.defer();

            $http.get('Departments/GetOrganisations', { params: filter }).then(function (response) {
                angular.forEach(response.data.rows, function (value, key) {
                    value.$$treeLevel = value.Level;
                });
                deferred.resolve(response.data.rows);
            }, function (response) {
                dialogService.showError(response.data, "Ошибка получения подразделений");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getDepartmentDetails = function (organisationUID, UID, parentUID) {
            var deferred = $q.defer();

            $http.get('Departments/GetDepartmentDetails', { params: {
                organisationId: organisationUID,
                id: UID,
                parentDepartmentId: parentUID
            }
            }).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                dialogService.showError(response.data, "Ошибка получения подразделения");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _getDepartmentDetailsMany = function (organisationUID, UIDs) {
            var deferred = $q.defer();

            $http.get('Departments/GetDepartmentDetailsMany', { params: {
                organisationId: organisationUID,
                ids: UIDs
            }
            }).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                dialogService.showError(response.data, "Ошибка получения подразделений");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _paste = function (organisationUID, parentUID, departments) {
            var deferred = $q.defer();

            if (parentUID) {
                departments[0].SelectedDepartment = { UID: parentUID };
            } else {
                departments[0].SelectedDepartment = null;
            }
            angular.forEach(departments, function (department) {
                department.Department.OrganisationUID = organisationUID;
            });

            $http.post('Departments/Paste', {
                departments: departments
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                dialogService.showError(response.data, "Ошибка вставки подразделения");
                deferred.reject();
            });

            return deferred.promise;
        };

        var _getDepartmentEmployeeList = function (filter) {
            var deferred = $q.defer();

            $http.get('Departments/GetDepartmentEmployeeList', {
                 params: {
                     departmentId: this.selectedDepartment.UID,
                     organisationId: this.selectedDepartment.OrganisationUID,
                     isWithDeleted: filter.LogicalDeletationType === "All",
                     chiefId: this.selectedDepartment.Model.ChiefUID
                 }
            }).then(function (response) {
                deferred.resolve(response.data.rows);
            }, function (response) {
                dialogService.showError(response.data, "Ошибка получения сотрудников подразделения");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _saveEmployeeDepartment = function (employee, selectedDepartmentUID) {
            var deferred = $q.defer();

            $http.post('Departments/SaveEmployeeDepartment', {
                employeeUID: employee.UID,
                departmentUID: selectedDepartmentUID,
                name: employee.Name
            }).then(function (response) {
                deferred.resolve(employee);
            }, function (response) {
                dialogService.showError(response.data, "Ошибка сохранения сотрудника подразделения");
                deferred.reject();
            });

            return deferred.promise;
        };

        var _saveDepartmentChief = function (department, employeeUID) {
            var deferred = $q.defer();

            $http.post('Departments/SaveDepartmentChief', {
                departmentUID: department.UID,
                employeeUID: employeeUID,
                name: department.NameData
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                dialogService.showError(response.data, "Ошибка установки руководителя");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _saveDepartment = function (department, isNew) {
            var deferred = $q.defer();

            if (department.photoData === "//:0") {
                department.photoData = null;
            }

            $http.post('Departments/DepartmentDetails', {
                departmentModel: department,
                isNew: isNew
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                dialogService.showError(response.data, "Ошибка сохранения подразделения");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _markDeleted = function (UID) {
            var deferred = $q.defer();

            $http.post('Departments/MarkDeleted', {
                uid: UID
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                dialogService.showError(response.data, "Ошибка удаления подразделения");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _restore = function (department) {
            var deferred = $q.defer();

            $http.post("Departments/Restore", {
                uid: department.UID
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                dialogService.showError(response.data, "Ошибка восстановления подразделения");
                deferred.reject();
            });

            return deferred.promise;
        };

        var _getChildEmployeeUIDs = function (UID) {
            var deferred = $q.defer();

            $http.get('Departments/GetChildEmployeeUIDs', { params: { departmentId: UID } }).then(function (response) {
                deferred.resolve(response.data);
            }, function (response) {
                dialogService.showError(response.data, "Ошибка получения сотрудников");
                deferred.reject();
            });

            return deferred.promise;
        };

        var _getDepartmentEmployees = function (UID) {
            var deferred = $q.defer();

            $http.get('Hr/GetDepartmentEmployees/' + UID).then(function (response) {
                deferred.resolve(response.data.Employees);
            }, function (response) {
                dialogService.showError(response.data, "Ошибка получения сотрудников");
                deferred.reject();
            });

            return deferred.promise;
        };



        return {
            selectedDepartment: null,
            reload: null,
            getDepartments: _getDepartments,
            getDepartmentEmployeeList: _getDepartmentEmployeeList,
            getDepartmentDetailsMany: _getDepartmentDetailsMany,
            saveEmployeeDepartment: _saveEmployeeDepartment,
            saveDepartmentChief: _saveDepartmentChief,
            saveDepartment: _saveDepartment,
            getDepartmentDetails: _getDepartmentDetails,
            markDeleted: _markDeleted,
            restore: _restore,
            paste: _paste,
            getChildEmployeeUIDs: _getChildEmployeeUIDs,
            getDepartmentEmployees: _getDepartmentEmployees
        }
    }]);
}());
