(function ()
{
    'use strict';

    var app = angular.module('canvasApp.controllers').controller('SignalrCtrl', [
        '$scope', 'signalrService', function ($scope, signalrService)
        {
           $scope.signalService = signalrService; 
        }
    ]);
}());