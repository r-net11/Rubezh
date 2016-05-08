import {Component, OnInit} from '@angular/core';
import {ROUTER_DIRECTIVES, Routes} from '@angular/router';
import {HashLocationStrategy} from
"@angular/common/index";
import { MdIcon, MdIconRegistry } from '@angular2-material/icon';
import { MD_SIDENAV_DIRECTIVES } from '@angular2-material/sidenav';
import { MdToolbar } from '@angular2-material/toolbar';
import { MdButton } from '@angular2-material/button';
import {HTTP_PROVIDERS} from '@angular/http';

//import {SignalrTestComponent} from './signalr/signalr-test.component';
import {HelloWorldComponent} from './hello/hello-world.component';

@Component({
	selector: 'gk-app',
	templateUrl: 'views/layout/layout.html',
	directives: [ROUTER_DIRECTIVES, MdIcon, MD_SIDENAV_DIRECTIVES, MdToolbar, MdButton],
	viewProviders: [MdIconRegistry],
	providers: [HTTP_PROVIDERS]
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
export class AppComponent implements OnInit
{
	constructor(mdIconRegistry: MdIconRegistry)
	{
		mdIconRegistry.registerFontClassAlias('fontawesome', 'fa');
	}
	ngOnInit()
	{
	}
}