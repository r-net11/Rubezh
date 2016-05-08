import {bootstrap} from "@angular/platform-browser-dynamic";
import {ROUTER_DIRECTIVES, ROUTER_PROVIDERS} from
"@angular/router";
import {Location, LocationStrategy, HashLocationStrategy} from
"@angular/common/index";
import {HTTP_PROVIDERS} from '@angular/http';
import {provide} from "@angular/core";
import 'rxjs/Rx';

import {AppComponent} from "./app.component";

bootstrap(AppComponent, [ROUTER_PROVIDERS, Location, HTTP_PROVIDERS, provide(LocationStrategy, { useClass: HashLocationStrategy })]);