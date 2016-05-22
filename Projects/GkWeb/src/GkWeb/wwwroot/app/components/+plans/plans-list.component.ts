import { Component, OnInit, Input } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes } from '@angular/router';
import { HTTP_PROVIDERS }    from '@angular/http';

import { DataService } from '../../shared/services/index';
import { PlanComponent  } from './plan.component';
import { PlanInfo } from './+models/index';

@Component({
	selector: '[gk-plans-list]',
	templateUrl: 'app/components/+plans/plans-list.component.html',
	styleUrls: ['app/components/+plans/plans-list.component.css'],
	directives: [ROUTER_DIRECTIVES, PlansListComponent, PlanComponent],
	providers: [HTTP_PROVIDERS, DataService]
})
	@Routes([
		{ path: '/:id', component: PlanComponent }
])
export class PlansListComponent implements OnInit
{
	errorMessage: string;
	@Input() plans: Array<PlanInfo>;
	@Input() isSubElement: boolean;

	constructor(
		private router: Router, private dataService: DataService)
	{
		
		
	}

	load()
	{
		this.dataService.getPlansList().subscribe(
			plans => this.plans = plans,
			error => this.errorMessage = <any>error);
	}

	ngOnInit()
	{
		if (this.isSubElement !== null && this.isSubElement === true)
		{
			return;
		} else
		{
			this.load();
		}		
	}
}