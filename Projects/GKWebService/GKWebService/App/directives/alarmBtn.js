(function () {
    'use strict';

    angular.module("gkApp")
        .directive("alarmBtn", function () {
            return function (scope, element, attributes) {
                var alarmType = scope[attributes["alarmBtn"]];
                var glowColor = attributes["glowColor"];
                var color = attributes["color"];
                var elem = angular.element("<div>");
                elem.addClass("alarmBtn");

                var highlightElem = angular.element('<div>');
                highlightElem.addClass("alarmBtnHighlight");

                var glowElem = angular.element('<div>');
                glowElem.addClass("alarmBtnGlow");

                elem.append(glowElem);
                element.append(elem);

                glowElem.after(highlightElem);
            }
        });
}());
