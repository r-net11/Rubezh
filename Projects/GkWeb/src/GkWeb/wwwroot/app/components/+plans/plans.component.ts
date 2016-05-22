import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes } from '@angular/router';
import { PlansListComponent } from './plans-list.component';
import { PlanComponent } from './plan.component';

 @Component({
	selector: '[gk-plans]',
	templateUrl: 'app/components/+plans/plans.component.html',
	directives: [ROUTER_DIRECTIVES, PlansListComponent]
})
 @Routes([
		 { path: '/:id', component: PlanComponent }
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