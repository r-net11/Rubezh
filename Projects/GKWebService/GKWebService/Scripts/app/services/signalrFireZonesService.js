(function () {
    'use strict';

    var app = angular.module('canvasApp.services')
        .factory('signalrFireZonesService', ['Hub', function (Hub) {

            var fireZonesUpdater = new Hub('fireZonesUpdater', {
                //client side methods
                listeners: {
                    'RefreshZoneState': function (imageBloom) {
                        $('td:nth-child(2) > img:nth-child(2)')[0].src = "data:image/gif;base64," + imageBloom;
                    }
                }
            });
            return {
                func: function () {
                }
            };
        }]);
}());
