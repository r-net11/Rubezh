import { Component, OnInit } from '@angular/core';
import { HTTP_PROVIDERS }    from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

import { DataService }       from '../services/data.service';

@Component({
	selector: 'nav-identity',
	templateUrl: 'app/shared/nav/user-identity.component.html',
	providers: [HTTP_PROVIDERS, DataService]
})
export class UserIdentityComponent implements OnInit
{
	constructor(private dataService: DataService) { }

	public userName: string;

	ngOnInit() { this.getUserName(); }
	
	getUserName()
	{
		this.dataService.getUserName().subscribe(res => this.userName = res);
	}

}