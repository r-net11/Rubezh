import {Injectable, Inject} from "@angular/core";
import { Http, Response } from '@angular/http';
import { Observable }     from 'rxjs/Observable';
import 'rxjs/add/operator/map';

@Injectable()
export class DataService
{
	constructor(private http: Http)
	{

	}

	getUserName()
	{
		return this.http.get('logon/GetUserName').map(res => res.text());
	}
}