import { bootstrap } from "@angular/platform-browser-dynamic";
import { ROUTER_PROVIDERS } from "@angular/router";
import { provide } from "@angular/core";
import { LocationStrategy, HashLocationStrategy } from "@angular/common";

//import {enableProdMode} from '@angular/core'

//enableProdMode();

import { AppComponent } from "./app.component";


bootstrap(AppComponent, [ROUTER_PROVIDERS, provide(LocationStrategy, { useClass: HashLocationStrategy })]);