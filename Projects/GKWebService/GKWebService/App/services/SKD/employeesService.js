(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('employeesService', ['$http', '$q', function ($http, $q) {
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
            }
        }
    }]);
}());
