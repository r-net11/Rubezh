import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes } from '@angular/router';
import { PlansComponent } from '../../+components/index';

@Component({
	selector: 'gk-nav',
	templateUrl: 'app/shared/nav/nav.component.html',
	directives: [ROUTER_DIRECTIVES, PlansComponent]
})
@Routes([
		{ path: '/plans', component: PlansComponent	}
])
export class NavComponent implements OnInit
{
	constructor(
		private _router: Router)
	{

	}

	ngOnInit()
	{
	}
}