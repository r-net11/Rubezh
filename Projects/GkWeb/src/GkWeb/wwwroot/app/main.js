System.register(["@angular/platform-browser-dynamic", "@angular/router", "@angular/common/index", "@angular/core", "./app.component", "./services/gk.service"], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var platform_browser_dynamic_1, router_1, index_1, core_1, app_component_1, gk_service_1;
    var channelConfig;
    return {
        setters:[
            function (platform_browser_dynamic_1_1) {
                platform_browser_dynamic_1 = platform_browser_dynamic_1_1;
            },
            function (router_1_1) {
                router_1 = router_1_1;
            },
            function (index_1_1) {
                index_1 = index_1_1;
            },
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (app_component_1_1) {
                app_component_1 = app_component_1_1;
            },
            function (gk_service_1_1) {
                gk_service_1 = gk_service_1_1;
            }],
        execute: function() {
            channelConfig = new gk_service_1.ChannelConfig();
            channelConfig.url = "http://localhost:5000/signalr";
            channelConfig.hubName = "EventHub";
            platform_browser_dynamic_1.bootstrap(app_component_1.AppComponent, [router_1.ROUTER_PROVIDERS, gk_service_1.GkService,
                core_1.provide(gk_service_1.SignalrWindow, { useValue: window }),
                core_1.provide("channel.config", { useValue: channelConfig }),
                core_1.provide(index_1.LocationStrategy, { useClass: index_1.HashLocationStrategy })]);
        }
    }
});
//# sourceMappingURL=main.js.map