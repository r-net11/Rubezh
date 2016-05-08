import {bootstrap} from "@angular/platform-browser-dynamic";
import {ROUTER_DIRECTIVES, ROUTER_PROVIDERS} from
"@angular/router";
import {LocationStrategy, HashLocationStrategy} from
"@angular/common/index";
import {provide} from "@angular/core";

import {AppComponent} from "./app.component";

bootstrap(AppComponent, [ROUTER_PROVIDERS, provide(LocationStrategy, { useClass: HashLocationStrategy })]);