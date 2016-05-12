import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes } from '@angular/router';

@Component({
	selector: 'gk-nav',
	templateUrl: 'app/shared/nav/nav.component.html',
	directives: [ROUTER_DIRECTIVES]
})
@Routes([
		//{ path: '/plans', component: PlansComponent	}
])
export class NavComponent implements OnInit
{
	constructor(
		private router: Router)
	{

	}

	onNavigateToPlansClick(event)
	{
		this.router.navigate(['/plans', { outlet: 'topOutlet' }]);
	}

	ngOnInit()
	{
	}
}