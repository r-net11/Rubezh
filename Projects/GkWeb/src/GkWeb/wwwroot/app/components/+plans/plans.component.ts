import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes } from '@angular/router';
import { HTTP_PROVIDERS } from '@angular/http';

import { PlansListComponent } from './plans-list.component';
import { PlanComponent } from './plan.component';


 @Component({
	selector: '[gk-plans]',
	templateUrl: 'app/components/+plans/plans.component.html',
	directives: [ROUTER_DIRECTIVES, PlansListComponent],
	providers: [HTTP_PROVIDERS]
})
 @Routes([
		 { path: '/:id', component: PlanComponent }
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