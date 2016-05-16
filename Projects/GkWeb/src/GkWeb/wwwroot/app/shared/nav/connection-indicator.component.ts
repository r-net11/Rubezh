/// <reference path="../../../typings/browser/ambient/jquery/index.d.ts" />
/// <reference path="../../../typings/browser/ambient/signalr/index.d.ts" />
import { Component, OnInit } from "@angular/core";
import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/map";

import { GkService } from "../services/gk.service";

@Component({
	selector: "nav-connection",
	templateUrl: "app/shared/nav/connection-indicator.component.html",
	styleUrls: ["app/shared/nav/connection-indicator.component.css"],
	providers: [GkService]
})
export class ConnectionIndicatorComponent implements OnInit {
	connectionState: ConnectionState;

	private started: boolean;
	private startedError: any;
	private channel = "connection";

	constructor(private gkService: GkService)
	{
		// Подписываемся на observables для SignalR
		//
		this.connectionState = new ConnectionState();
		this.gkService.connectionState
			.subscribe((state) => {
				console.info("Connection state has changed.");
				this.connectionState.fromState(state);
			});

		//this.gkService.error.subscribe(
		//	(error: any) => {
		//		console.warn(error);
		//	},
		//	(error: any) => {
		//		console.error("errors$ error", error);
		//	}
		//);

		//this.gkService.starting.subscribe(
		//	() => {
		//		console.log(
		//			"signalr service has been started");
		//	},
		//	() => {
		//		console.warn(
		//			"signalr service failed to start!");
		//	}
		//);

	}

	ngOnInit() {
		console.log("Запускается Gk Service.");
		this.gkService.start();
	}

}

class ConnectionState {
	Text: string;
	Class: string;

	constructor() {
		this.Text = "Подключение отсутствует";
		this.Class = "disconnected";
	}

	fromState(state: number) {
		switch (state) {
		case SignalR.ConnectionState.Connecting.valueOf():
		{
			this.Text = "Выполняется подключение к серверу";
			this.Class = "connecting";
			break;
		}
		case SignalR.ConnectionState.Connected.valueOf():
		{
			this.Text = "Подключено к серверу";
			this.Class = "connected";
			break;
		}
		case SignalR.ConnectionState.Disconnected.valueOf():
		{
			this.Text = "Подключение отсутствует";
			this.Class = "disconnected";
			break;
		}
		case SignalR.ConnectionState.Reconnecting.valueOf():
		{
			this.Text = "Выполняется попытка переподключения к серверу";
			this.Class = "reconnecting";
			break;
		}
		default:
		{
			this.Text = "Подключение отсутствует";
			this.Class = "disconnected";
			break;
		}
		}
	}
}