(function () {
    'use strict';

    var app = angular.module('gkApp.services');
    app.constant('constants', {
        gkObject: {
            device: {
                type: 0,
                detailsTemplate: 'Devices/DeviceDetails',
                detailsCtrl: 'devicesDetailsCtrl',
                controlPermission: 'Oper_Device_Control'
            },
            zone: {
                type: 1,
                detailsTemplate: 'FireZones/FireZonesDetails',
                detailsCtrl: 'fireZonesDetailsCtrl',
                controlPermission: 'Oper_Zone_Control'
            },
            direction: {
                type: 2,
                detailsTemplate: 'Directions/DirectionDetails',
                detailsCtrl: 'directionDetailsCtrl',
                controlPermission: 'Oper_Directions_Control'
            },
            pumpStation: {
                type: 3,
                detailsTemplate: 'PumpStations/PumpStationDetails',
                detailsCtrl: 'pumpStationDetailsCtrl',
                controlPermission: 'Oper_NS_Control'
            },
            mpt: {
                type: 4,
                detailsTemplate: 'MPTs/MPTDetails',
                detailsCtrl: 'mptsDetailsCtrl',
                controlPermission: 'Oper_MPT_Control'
            },
            delay: {
                type: 5,
                detailsTemplate: 'Delays/DelayDetails',
                detailsCtrl: 'delayDetailsCtrl',
                controlPermission: 'Oper_Delay_Control'
            },
            pim: {
                type: 6,
                detailsTemplate: '',
                detailsCtrl: '',
                controlPermission: ''
            },
            guardZone: {
                type: 7,
                detailsTemplate: 'GuardZones/GuardZoneDetails',
                detailsCtrl: 'guardZoneDetailsCtrl',
                controlPermission: 'Oper_GuardZone_Control'
            },
            code: {
                type: 8,
                detailsTemplate: '',
                detailsCtrl: '',
                controlPermission: 'Oper_Device_Control'
            },
            door: {
                type: 9,
                detailsTemplate: 'Doors/DoorDetails',
                detailsCtrl: 'doorDetailsCtrl',
                controlPermission: 'Oper_Door_Control'
            },
            skdZone: {
                type: 10,
                detailsTemplate: '',
                detailsCtrl: '',
                controlPermission: ''
            },
			restart: {
				type: 11,
				detailsTemplate: 'Home/RestartDetails',
				detailsCtrl: 'restartDetailsCtrl'
			}
        },
        emptyGuid: "00000000-0000-0000-0000-000000000000"
    });
}());
