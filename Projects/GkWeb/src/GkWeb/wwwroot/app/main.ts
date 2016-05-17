import { bootstrap } from "@angular/platform-browser-dynamic";
import { ROUTER_PROVIDERS } from "@angular/router";
import { provide } from "@angular/core";
import { LocationStrategy, HashLocationStrategy } from "@angular/common";

import { GkService } from "./shared/index";
import { AppComponent } from "./app.component";

//import {enableProdMode} from '@angular/core'

//enableProdMode();



bootstrap(AppComponent, [ROUTER_PROVIDERS, provide(LocationStrategy, { useClass: HashLocationStrategy }), GkService]);