(function () {
    'use strict';

    angular.module("gkApp")
        .directive('modaless', function ($document) {
            return function(scope, element) {
                element = element.parent().parent();

                var elementModal = element.parent();

                elementModal.css({
                    'pointer-events': 'none'
                });

                element.css({
                    position: 'fixed',
                    cursor: 'move',
                    'pointer-events': 'initial'
                });

            };
        });
}());


