(function () {
    'use strict';

    angular.module("gkApp")
        .directive('modalDraggable', function ($document) {
            return function (scope, element) {
                element = element.parent().parent();
                element.draggable();
            };
        });
}());


