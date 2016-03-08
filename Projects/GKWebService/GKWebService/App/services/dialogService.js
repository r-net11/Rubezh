(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('dialogService', ['$uibModal', function ($uibModal) {
        var openedWindows = [];

        function indexOf(arr, item) {
            for (var i = 0; i < arr.length; i++) {
                if (arr[i] === item) {
                    return i;
                }
            }
            return -1;
        };

        function tryRemove(arr, item) {
            var index = indexOf(arr, item);
            if (index !== -1) {
                arr.splice(index, 1);
            }
        };

        return {
            showWindow: function (costants, entity) {
                var openedIndex = indexOf(openedWindows, entity.UID);

                if (openedIndex === -1) {
                    var modalInstance = $uibModal.open({
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

                    modalInstance.result.then(function () {
                        tryRemove(openedWindows, entity.UID);
                    },
                    function () {
                        tryRemove(openedWindows, entity.UID);
                    });

                    openedWindows.push(entity.UID);
                }
            }
        }
    }]);
}());
