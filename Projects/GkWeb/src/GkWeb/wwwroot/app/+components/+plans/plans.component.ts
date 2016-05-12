import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes } from '@angular/router';

@Component({
	selector: 'gk-plans',
	templateUrl: 'app/+components/+plans/plans.component.html'
	//styleUrls: ['app/app.component.css'],
	//directives: [ROUTER_DIRECTIVES]
})
@Routes([
	//{ path: '/hello-world', component: HelloWorldComponent },
	//{ path: '/tasks', component: TasksComponent }
])
export class PlansComponent implements OnInit
{
	constructor(
		private _router: Router)
	{

	}

	ngOnInit()
	{
	}
}