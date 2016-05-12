import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes } from '@angular/router';

import { NavComponent } from './shared/index';
import { PlansComponent } from './+components/index';

@Component({
	selector: 'gk-app',
	templateUrl: 'app/app.component.html',
	styleUrls: ['app/app.component.css'],
	directives: [ROUTER_DIRECTIVES, NavComponent]
})
@Routes([
		{ path: '/plans', component: PlansComponent }
		//{ path: '/hello-world', component: HelloWorldComponent },
		//{ path: '/tasks', component: TasksComponent }
])
export class AppComponent implements OnInit
{
	constructor(
		private _router: Router)
	{

	}

	openHello(event)
	{
		event.preventDefault();
		this._router.navigate(['/hello-world']);
		this._router.navigate(
	}

	ngOnInit()
	{
	}
}