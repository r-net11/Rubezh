/// <reference path="../../../typings/browser/ambient/signalr/index.d.ts" />
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { GkService } from '../services/gk.service';

@Component({
	selector: 'nav-connection',
	templateUrl: 'app/shared/nav/connection-indicator.component.html',
	providers: [GkService]
})
export class ConnectionIndicatorComponent implements OnInit
{
	// An internal "copy" of the connection state stream used because
    //  we want to map the values of the original stream. If we didn't 
    //  need to do that then we could use the service's observable 
    //  right in the template.
    //   
    connectionState: Observable<string>;

	private started: boolean;
	private startedError: any;
	private channel = "connection";

	constructor(private gkService: GkService)
	{
		// Let's wire up to the signalr observables
        //
		this.connectionState = this.gkService.connectionState
            .map((state: SignalR.ConnectionState) => { return state.toString(); });

        this.gkService.error.subscribe(
            (error: any) =>
			{
				console.warn(error);
			},
            (error: any) =>
			{
				console.error("errors$ error", error);
			}
        );

        // Wire up a handler for the starting$ observable to log the
        //  success/fail result
        //
        this.gkService.starting.subscribe(
            () =>
			{
				console.log(
					"signalr service has been started");
			},
            () =>
			{
				console.warn(
					"signalr service failed to start!");
			}
        );

	}	

	ngOnInit()
	{
        // Start the connection up!
        //
        console.log("Starting the channel service");

        this.gkService.start();
    }

	//getUserName()
	//{
	//	this.dataService.getUserName().subscribe(res => this.userName = res);
	//}

}