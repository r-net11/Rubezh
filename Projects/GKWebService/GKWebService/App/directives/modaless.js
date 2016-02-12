(function () {
    'use strict';

    angular.module("gkApp")
        .directive('modaless', function ($document) {
            return function(scope, element) {
                var elementModal = element.parent().parent().parent();
                elementModal.css({
                    'pointer-events': 'none'
                });
            };
        });
}());


