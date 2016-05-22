import { Component, OnInit, Input } from '@angular/core';
import { ROUTER_DIRECTIVES, Router } from '@angular/router';

import { DataService } from '../../shared/services/index';
import { PlanInfo } from './+models/index';

@Component({
	selector: '[gk-plans-list]',
	templateUrl: 'app/components/+plans/plans-list.component.html',
	styleUrls: ['app/components/+plans/plans-list.component.css'],
	directives: [ROUTER_DIRECTIVES, PlansListComponent],
	providers: [DataService]
})
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