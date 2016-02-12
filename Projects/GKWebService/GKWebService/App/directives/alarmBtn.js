(function () {
    'use strict';

    angular.module("gkApp")
        .directive("alarmBtn", function () {
            return {
                link: function (scope, element, attributes) {
                    scope.alarmType = scope[attributes["alarmBtn"]];

                    scope.alarmIcon = "Content/Image/Icon/Alarms/" + attributes["alarmIcon"] + ".png";

                    var glowColor = attributes["glowColor"] || "#FFFFFF";
                    var dec = parseInt(glowColor.replace('#', ''), 16);
                    var template = 'rgba(' + ((dec >> 16) & 255) + ', ' + ((dec >> 8) & 255) + ', ' + (dec & 255) + ', alpha)';
                    scope.glowColorFrom = template.replace('alpha', '0.68');
                    scope.glowColorTo = template.replace('alpha', '0');
                    if (angular.isDefined(attributes["count"])) {
                        attributes.$observe('count', function (newValue) {
                            if (newValue === '0') {
                                scope.count = '';
                            } else 
                            {
                                scope.count = newValue;
                            }
                        });
                    }
                },
                restrict: "A",
                replace: true,
                template: function () {
                    return "<div class='alarmBtn' ng-style='{\"background-image\": \"url(\" + alarmIcon + \")\"}' >" +
                        "<div class='alarmBtnGlow' " +
                        "style='background-image: -webkit-radial-gradient(0% 100%, circle, {{glowColorFrom}} 0%, {{glowColorTo}} 100%);" +
                        "background-image:   -o-radial-gradient(0% 100%, circle, {{glowColorFrom}} 0%, {{glowColorTo}} 100%);" +
                        "background-image: -moz-radial-gradient(0% 100%, circle, {{glowColorFrom}} 0%, {{glowColorTo}} 100%);" +
                        "background-image:  -ms-radial-gradient(0% 100%, circle, {{glowColorFrom}} 0%, {{glowColorTo}} 100%);" +
                        "background-image:      radial-gradient(0% 100%, circle, {{glowColorFrom}} 0%, {{glowColorTo}} 100%)'" +
                        " />" +
                        "<div class='alarmBtnCount'><span>{{count}}</span></div>" +
                        "<div class='alarmBtnHighlight' />" +
                        "</div>";
                }
            }
        });
}());
