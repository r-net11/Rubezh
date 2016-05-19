import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes } from '@angular/router';
import { HTTP_PROVIDERS }    from '@angular/http';

import { DataService } from '../../shared/services/index';
import { PlanInfo  } from './+models/planInfo.model';

@Component({
	selector: 'gk-plans-list',
	templateUrl: 'app/components/+plans/plans-list.component.html',
	styleUrls: ['app/components/+plans/plans-list.component.css'],
	directives: [ROUTER_DIRECTIVES],
	providers: [HTTP_PROVIDERS, DataService]
})
@Routes([
])
export class PlansListComponent implements OnInit
{
	errorMessage: string;
	plans: Array<PlanInfo>;

	constructor(
		private router: Router, private dataService: DataService)
	{
		this.load();
		
	}

	load()
	{
		this.dataService.getPlansList().subscribe(
			plans => this.plans = plans,
			error => this.errorMessage = <any>error);
	}

	ngOnInit()
	{
		
	}
}