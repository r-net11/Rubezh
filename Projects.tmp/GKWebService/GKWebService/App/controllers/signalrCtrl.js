(function ()
{
    'use strict';

    var app = angular.module('gkApp.controllers').controller('SignalrCtrl', [
        '$scope', 'signalrService', function ($scope, signalrService)
        {
           $scope.signalService = signalrService; 
        }
    ]);
}());