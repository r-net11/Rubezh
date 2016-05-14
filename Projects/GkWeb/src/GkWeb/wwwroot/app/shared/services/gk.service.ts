/// <reference path="../../../typings/browser/ambient/signalr/index.d.ts" />
import {Injectable} from "@angular/core";

@Injectable()
export class GkService
{
	connection: SignalR.Connection;
	hubConnection: SignalR.Hub.Connection;
	hubProxy: SignalR.Hub.Proxy;

	constructor() {
		this.hubConnection.url = "http://localhost:5000/signalr";
		this.hubProxy = this.hubConnection.createHubProxy("gkHub");
		this.connection.connectionSlow(() => { });
		this.connection.disconnected(() => { });
		this.connection.error(() => { });
		this.connection.reconnected(() => { });
		this.connection.reconnecting(() => { });
		this.connection.stateChanged((change) =>
		{
			if (change.newState === change.oldState) {
				return;
			}
				switch (change.newState) {
				case SignalR.ConnectionState.Connected:
					{
					}
				case SignalR.ConnectionState.Connecting:
					{
					}
				case SignalR.ConnectionState.Disconnected:
					{
					}
				case SignalR.ConnectionState.Reconnecting:
					{
					}
				default:
				}
			}
		);

	}

	start() {
		this.hubConnection.start().done(() =>
		{
			//this.startingSubject.next(null);
		})
            .fail((error: any) =>
			{
                //this.startingSubject.error(error);
            });
	}
}