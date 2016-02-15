(function () {

    var app = angular.module('gkApp.services')
    .factory('signalrDevicesService',['Hub', 'broadcastService',  function(Hub, broadcastService){
        var devicesHub = new Hub('devicesUpdater', {
            listeners: {
                'devicesUpdate': function (device) {
                    broadcastService.send('devicesChanged', device);
                }
            },
            queryParams: {
                'token': 'exampletoken'
            },
            errorHandler: function (error) {
                console.error(error);
            },

            stateChanged: function (state) {
                switch (state.newState) {
                    case $.signalR.connectionState.connecting:
                        //your code here
                        break;
                    case $.signalR.connectionState.connected:
                        break;
                    case $.signalR.connectionState.reconnecting:
                        //your code here
                        break;
                    case $.signalR.connectionState.disconnected:
                        //your code here
                        break;
                }
            }
        });
        return {};
    }]);
}());