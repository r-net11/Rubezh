import { Component, OnInit, Input } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes, RouteSegment } from '@angular/router';
import { HTTP_PROVIDERS }    from '@angular/http';

@Component({
	selector: '[gk-plan]',
	templateUrl: 'app/components/+plans/plan.component.html',
	styleUrls: ['app/components/+plans/plan.component.css'],
	directives: [ROUTER_DIRECTIVES],
	providers: [HTTP_PROVIDERS]
})
@Routes([
])
export class PlanComponent implements OnInit
{
	id: string;

	constructor(
		private router: Router)
	{
		
	}

	routerOnActivate(curr: RouteSegment)
	{
		this.id = curr.getParam('id');
	}

	load()
	{
		
	}

	ngOnInit()
	{
		
	}
}