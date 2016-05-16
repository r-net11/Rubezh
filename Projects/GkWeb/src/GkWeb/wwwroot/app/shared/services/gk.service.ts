/// <reference path="../../../typings/browser/ambient/signalr/index.d.ts" />
import {Injectable} from "@angular/core";
import {Observable} from 'rxjs/Observable';
import {Subject} from 'rxjs/Subject';

@Injectable()
export class GkService
{
	/**
     * starting - это observable, оповещающий о готовности подключения
	 * SignalR. При успешном подключении, этот поток выдаст значение.
     */
    starting: Observable<any>;

	/**
     * connectionState предоставлят текущий статус низлежащего
     * подключения SignalR как observable поток.
     */
    connectionState: Observable<SignalR.ConnectionState>;

    /**
     * error$ предоставляет поток любых сообщений об ошибках, 
     * которые возникают в подключении SignalR.
     */
    error: Observable<string>;

	// Эти поля используются для трансляции в соответствующие публичные Observable
	//
    private connectionStateSubject = new Subject<SignalR.ConnectionState>();
    private startingSubject = new Subject<any>();
    private errorSubject = new Subject<string>();

	// Эти поля используются для отслеживания внутреннего состоянмя SignalR.
	//
	private hubConnection: SignalR.Hub.Connection;
	private hubProxy: SignalR.Hub.Proxy;

	constructor()
	{
		// Настройка observables
        //
        this.connectionState = this.connectionStateSubject.asObservable();
        this.error = this.errorSubject.asObservable();
        this.starting = this.startingSubject.asObservable();

		// Создаем JavaScript-прокси для хаба
		//
		this.hubConnection = $.connection.hub;
		//this.hubProxy = this.hubConnection.createHubProxy("gkHub");


		// Задаем обработчики событий подключения
		//
        this.hubConnection.stateChanged((change: any) =>
		{
			if (change.newState === change.oldState)
			{
				return;
			}
			console.info("Signalr connection state changed.");
		// Проталкиваем новое состояние в субъект
		//
		this.connectionStateSubject.next(change.newState);
		});
		//this.hubConnection.error(this.onConnectionError);
		this.hubConnection.connectionSlow(() => { console.info("Signalr connection is slow.");});
		this.hubConnection.disconnected(() => { console.info("Signalr disconnected.") });
		//this.hubConnection.error((e) => { console.error("Signalr connection error." + e.message) });
		this.hubConnection.reconnected(() => { console.info("Signalr connection reconnected.")  });
		this.hubConnection.reconnecting(() => { console.info("Signalr connection reconnecting.") });
	}

	private onConnectionError(error: SignalR.ConnectionError)
	{
		// Проталкиваем сообщение в субъект
		//
		this.errorSubject.next(error.message);
	}

	private onStateChanged(change: SignalR.StateChanged)
	{
		if (change.newState === change.oldState)
		{
			return;
		}
		// Проталкиваем новое состояние в субъект
		//
		//this.connectionStateSubject.next(change.newState);
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