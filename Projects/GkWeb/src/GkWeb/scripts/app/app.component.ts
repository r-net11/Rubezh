/// <reference path="../../node_modules/angular2/core.d.ts" />
import {Component, OnInit} from 'angular2/core';
import {RouteConfig, ROUTER_DIRECTIVES, HashLocationStrategy} from 'angular2/router';

//import {SignalrTestComponent} from './signalr/signalr-test.component';
import {HelloWorldComponent} from './hello/hello-world.component';

@Component({
	selector: 'gk-app',
	templateUrl: 'views/layout/layout.html',
	directives: [ROUTER_DIRECTIVES]
})
@RouteConfig([

	//{
	//	path: 'signalr-test',
	//	name: 'SignalrTest',
	//	component: SignalrTestComponent,
	//	useAsDefault: true
	//},

	{ path: '/hello-world', name: 'HelloWorld', component: HelloWorldComponent }
])
export class AppComponent implements OnInit {
	ngOnInit()
	{
		// init winjs menu
		WinJS.UI.processAll().done(() => {
			var splitView = document.querySelector(".splitView").winControl;
		});
	}
}