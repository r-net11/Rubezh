import {Component, OnInit}   from '@angular/core';
import {Router} from '@angular/router';

@Component({
	template: `
    <button (click)="onSayHello()">Say hello!</button>
	<input [(ngModel)]="helloText">
	<h2>{{helloMessage}}</h2>
  `
})
export class HelloWorldComponent implements OnInit
{
	constructor(
		private _router: Router) {
		
	}

	helloText: string;
	helloMessage: string;

	onSayHello() {
		this.helloMessage = this.helloText;
	}

	ngOnInit() {
		this.helloMessage = '';
		this.helloText = 'Hello from routed component!';
	}
}