import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES, Router, Routes } from '@angular/router';

import { UserIdentityComponent } from './user-identity.component';


@Component({
	selector: 'gk-nav',
	templateUrl: 'app/shared/nav/nav.component.html',
	directives: [ROUTER_DIRECTIVES, UserIdentityComponent]
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

	ngOnInit()
	{
	}
}