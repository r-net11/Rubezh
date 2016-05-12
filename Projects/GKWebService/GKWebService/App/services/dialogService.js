(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.factory('dialogService', ['$uibModal', '$window', function ($uibModal, $window) {
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

        var _showWindow = function(costants, entity) {
            var openedIndex = indexOf(openedWindows, entity.UID);

            if (openedIndex === -1) {
                var modalInstance = $uibModal.open({
                    animation: false,
                    templateUrl: costants.detailsTemplate,
                    controller: costants.detailsCtrl,
                    backdrop: false,
                    size: 'rbzh',
                    resolve: {
                        entity: function() {
                            return entity;
                        }
                    }
                });

                modalInstance.result.then(function() {
                        tryRemove(openedWindows, entity.UID);
                    },
                    function() {
                        tryRemove(openedWindows, entity.UID);
                    });

                openedWindows.push(entity.UID);
            }
        };

        var _showConfirm = function (message) {
            return $window.confirm(message);
        };

        var _showError = function (data, message) {
            if (message) {
                message = message + ". ";
            } else {
                message = "";
            }
            message = message + (data ? data.errorText : "");
            return $window.alert(message);
        };

        return {
            showWindow: _showWindow,
            showConfirm: _showConfirm,
            showError: _showError
        }
    }]);
}());
