(function () {

    var app = angular.module('gkApp.services')
        .factory('signalrAlarmsService',['Hub', 'broadcastService',  function(Hub, broadcastService){
            var devicesHub = new Hub('alarmsUpdater', {
                listeners: {
                    'updateAlarms': function (data) {
                        broadcastService.send('alarmsChanged', data);
                    }
                }
            });
            return {};
        }]);
}());