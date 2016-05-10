import {bootstrap} from "@angular/platform-browser-dynamic";
import {ROUTER_DIRECTIVES, ROUTER_PROVIDERS} from
"@angular/router";
import {LocationStrategy, HashLocationStrategy} from
"@angular/common/index";
import {provide} from "@angular/core";

import {AppComponent} from "./app.component";
import {GkService, ChannelConfig, SignalrWindow} from "./services/gk.service";

let channelConfig = new ChannelConfig();
channelConfig.url = "http://localhost:5000/signalr";
channelConfig.hubName = "EventHub";

bootstrap(AppComponent, [ROUTER_PROVIDERS, GkService,
    provide(SignalrWindow, { useValue: window }),
    provide("channel.config", { useValue: channelConfig }),
	provide(LocationStrategy, { useClass: HashLocationStrategy })]);