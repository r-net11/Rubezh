import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes } from '@angular/router';

import { DataService } from '../../shared/services/index';

@Component({
	selector: 'gk-plans',
	templateUrl: 'app/components/+plans/plans-list.component.html',
	styleUrls: ['app/components/+plans/plans-list.component.html.css'],
	directives: [ROUTER_DIRECTIVES]
})
@Routes([
	//{ path: '/hello-world', component: HelloWorldComponent },
	//{ path: '/tasks', component: TasksComponent }
])
export class PlansComponent implements OnInit
{
	constructor(
		private router: Router, private dataService: DataService)
	{

	}

	loadPlanList()
	{
	}

	ngOnInit()
	{
	}
}