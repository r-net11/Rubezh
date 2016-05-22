import { Component, OnInit, Input } from '@angular/core';
import { Router, Routes, RouteSegment } from '@angular/router';

import { DataService } from '../../shared/services/index';

@Component({
	selector: '[gk-plan]',
	templateUrl: 'app/components/+plans/plan.component.html',
	styleUrls: ['app/components/+plans/plan.component.css'],
	providers: [ DataService ]
})
@Routes([
])
export class PlanComponent implements OnInit
{
	id: string;

	constructor(
		private router: Router, private dataService: DataService)
	{
		
	}

	routerOnActivate(curr: RouteSegment)
	{
		this.id = curr.getParam('id');
	}

	load()
	{
		//this.dataService.getPlansList().subscribe(
		//	plans => this.plans = plans,
		//	error => this.errorMessage = <any>error);
	}

	ngOnInit()
	{
		
	}
}