(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.constant('constants', {
        gkObjectType: {
            device: 0,
            zone: 1,
            direction: 2,
            pumpStation: 3,
            mpt: 4,
            delay: 5,
            pim: 6,
            guardZone: 7,
            code: 8,
            door: 9,
            skdZone: 10
        }
    });
}());
