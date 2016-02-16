(function () {
        angular.module('gkApp.services').factory('signalrDoorsService', ['Hub', 'broadcastService', function (Hub, broadcastService) {
        var doorsUpdater = new Hub('doorsUpdater', {
            listeners: {
                'doorUpdate': function (door) {
                    broadcastService.send('doorChanged', door);
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