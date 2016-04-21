/// <reference path="../../node_modules/angular2/core.d.ts" />
import {Component} from 'angular2/core';
import {RouteConfig, ROUTER_DIRECTIVES} from 'angular2/router';

//import {SignalrTestComponent} from './signalr/signalr-test.component';
import {HelloWorldComponent} from './hello/hello-world.component';

@Component({
	selector: 'my-app',
	template: `
    <h1 class="title">Component Router</h1>
    <nav>
      <a [routerLink]="['HelloWorld']">Hello World!</a>
    </nav>
    <router-outlet></router-outlet>`,
	directives: [ROUTER_DIRECTIVES]
})
@RouteConfig([

	//{
	//	path: '/signalr-test',
	//	name: 'SignalrTest',
	//	component: SignalrTestComponent,
	//	useAsDefault: true
	//},

	{ path: '/hello-world', name: 'HelloWorld', component: HelloWorldComponent }
])
export class AppComponent { }