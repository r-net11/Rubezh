import {Component, OnInit} from '@angular/core';
import {ROUTER_DIRECTIVES, Routes} from '@angular/router';
import {HashLocationStrategy} from
"@angular/common/index";

//import {SignalrTestComponent} from './signalr/signalr-test.component';
import {HelloWorldComponent} from './hello/hello-world.component';

@Component({
	selector: 'gk-app',
	templateUrl: 'views/layout/layout.html',
	directives: [ROUTER_DIRECTIVES]
})
@Routes([

	//{
	//	path: 'signalr-test',
	//	name: 'SignalrTest',
	//	component: SignalrTestComponent,
	//	useAsDefault: true
	//},

		{ path: '/hello-world', component: HelloWorldComponent }
])
export class AppComponent implements OnInit {
	ngOnInit()
	{
		// init winjs menu
		//WinJS.UI.processAll().done(() => {
		//	var splitView = document.querySelector(".splitView").winControl;
		//});
	}
}