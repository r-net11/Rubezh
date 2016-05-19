import {Injectable, Inject} from "@angular/core";
import { Http, Response } from '@angular/http';
import { Observable }     from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { PlanInfo } from '../../components/index';

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

	getPlansList(): Observable<PlanInfo[]>
	{
		return this.http.get('api/plans')
			.map(this.extractData)
			.catch(this.handleError);
	}

	private extractData(res: Response)
	{
		let body = res.json();
		return body || {};
	}
	private handleError(error: any)
	{
		// In a real world app, we might use a remote logging infrastructure
		// We'd also dig deeper into the error to get a better message
		let errMsg = error.message || error.statusText || 'Server error';
		console.error(errMsg); // log to console instead
		return Observable.throw(errMsg);
	}
}