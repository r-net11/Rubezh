System.register(["@angular/platform-browser-dynamic", "@angular/router", "@angular/common/index", '@angular/http', "@angular/core", "./app.component"], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var platform_browser_dynamic_1, router_1, index_1, http_1, core_1, app_component_1;
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
            function (http_1_1) {
                http_1 = http_1_1;
            },
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (app_component_1_1) {
                app_component_1 = app_component_1_1;
            }],
        execute: function() {
            platform_browser_dynamic_1.bootstrap(app_component_1.AppComponent, [router_1.ROUTER_PROVIDERS, index_1.Location, http_1.HTTP_PROVIDERS, core_1.provide(index_1.LocationStrategy, { useClass: index_1.HashLocationStrategy })]);
        }
    }
});
//# sourceMappingURL=main.js.map