(function () {
    'use strict';

    /*Для сенсорных устройств сброс состояния "курсор внутри"*/
    angular.module("gkApp")
        .directive('mobileHoverFix', function () {
            return function (scope, element) {
                element.find(".btn-primary")
                    .on("touchstart", function () {
                        $(this).addClass("mobileHoverFix");
                    });
            };
        });
}());


