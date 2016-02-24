(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('dialogService', ['$uibModal', function ($uibModal) {
        return {
            showWindow: function (costants, entity) {
                $uibModal.open({
                    animation: false,
                    templateUrl: costants.detailsTemplate,
                    controller: costants.detailsCtrl,
                    backdrop: false,
                    size: 'rbzh',
                    resolve: {
                        entity: function () {
                            return entity;
                        }
                    }
                });
            }
        }
    }]);
}());
