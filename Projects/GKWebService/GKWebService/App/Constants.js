(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.constant('constants', {
        gkObject: {
            device: {
                type: 0,
                detailsTemplate: 'Devices/DeviceDetails',
                detailsCtrl: 'devicesDetailsCtrl'
            },
            zone: {
                type: 1,
                detailsTemplate: 'FireZones/FireZonesDetails',
                detailsCtrl: 'fireZonesDetailsCtrl'
            },
            direction: {
                type: 2,
                detailsTemplate: 'Directions/DirectionDetails',
                detailsCtrl: 'directionDetailsCtrl'
            },
            pumpStation: {
                type: 3,
                detailsTemplate: 'PumpStations/PumpStationDetails',
                detailsCtrl: 'pumpStationDetailsCtrl'
            },
            mpt: {
                type: 4,
                detailsTemplate: 'MPTs/MPTDetails',
                detailsCtrl: 'mptsDetailsCtrl'
            },
            delay: {
                type: 5,
                detailsTemplate: 'Delays/DelayDetails',
                detailsCtrl: 'delayDetailsCtrl'
            },
            pim: {
                type: 6,
                detailsTemplate: '',
                detailsCtrl: ''
            },
            guardZone: {
                type: 7,
                detailsTemplate: 'GuardZones/GuardZoneDetails',
                detailsCtrl: 'guardZoneDetailsCtrl'
            },
            code: {
                type: 8,
                detailsTemplate: '',
                detailsCtrl: ''
            },
            door: {
                type: 9,
                detailsTemplate: 'Doors/DoorDetails',
                detailsCtrl: 'doorDetailsCtrl'
            },
            skdZone: {
                type: 10,
                detailsTemplate: '',
                detailsCtrl: ''
            },
			restart: {
				type: 11,
				detailsTemplate: 'Home/RestartDetails',
				detailsCtrl: 'restartDetailsCtrl'
			}
        }
    });
}());
