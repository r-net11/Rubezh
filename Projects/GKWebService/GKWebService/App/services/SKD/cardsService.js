(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('cardsService', ['$http', '$q', 'dialogService', function ($http, $q, dialogService) {
        var _getCards = function(filter) {
            var deferred = $q.defer();

            $http.get('Cards/GetOrganisations', { params: filter }).then(function (response) {
                angular.forEach(response.data.rows, function (value, key) {
                    value.$$treeLevel = value.Level;
                });
                deferred.resolve(response.data.rows);
            }, function (response) {
                dialogService.showError(response.data, "Ошибка получения пропусков");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        var _markDeleted = function (UID) {
            var deferred = $q.defer();

            $http.post('Cards/MarkDeleted', {
                uid: UID
            }).then(function (response) {
                deferred.resolve();
            }, function (response) {
                dialogService.showError(response.data, "Ошибка удаления должности");
                deferred.reject();
            });

            return deferred.promise;
        };
    
        return {
            selectedCard: null,
            reload: null,
            getCards: _getCards,
            markDeleted: _markDeleted
        }
    }]);
}());
