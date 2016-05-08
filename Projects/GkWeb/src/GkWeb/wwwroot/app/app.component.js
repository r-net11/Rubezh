System.register(['@angular/core', '@angular/router', '@angular2-material/icon', '@angular2-material/sidenav', '@angular2-material/toolbar', '@angular2-material/button', '@angular/http', './hello/hello-world.component'], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var core_1, router_1, icon_1, sidenav_1, toolbar_1, button_1, http_1, hello_world_component_1;
    var AppComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (router_1_1) {
                router_1 = router_1_1;
            },
            function (icon_1_1) {
                icon_1 = icon_1_1;
            },
            function (sidenav_1_1) {
                sidenav_1 = sidenav_1_1;
            },
            function (toolbar_1_1) {
                toolbar_1 = toolbar_1_1;
            },
            function (button_1_1) {
                button_1 = button_1_1;
            },
            function (http_1_1) {
                http_1 = http_1_1;
            },
            function (hello_world_component_1_1) {
                hello_world_component_1 = hello_world_component_1_1;
            }],
        execute: function() {
            AppComponent = (function () {
                function AppComponent(mdIconRegistry) {
                    mdIconRegistry.registerFontClassAlias('fontawesome', 'fa');
                }
                AppComponent.prototype.ngOnInit = function () {
                };
                AppComponent = __decorate([
                    core_1.Component({
                        selector: 'gk-app',
                        templateUrl: 'views/layout/layout.html',
                        directives: [router_1.ROUTER_DIRECTIVES, icon_1.MdIcon, sidenav_1.MD_SIDENAV_DIRECTIVES, toolbar_1.MdToolbar, button_1.MdButton],
                        viewProviders: [icon_1.MdIconRegistry],
                        providers: [http_1.HTTP_PROVIDERS]
                    }),
                    router_1.Routes([
                        //{
                        //	path: 'signalr-test',
                        //	name: 'SignalrTest',
                        //	component: SignalrTestComponent,
                        //	useAsDefault: true
                        //},
                        { path: '/hello-world', component: hello_world_component_1.HelloWorldComponent }
                    ]), 
                    __metadata('design:paramtypes', [icon_1.MdIconRegistry])
                ], AppComponent);
                return AppComponent;
            }());
            exports_1("AppComponent", AppComponent);
        }
    }
});
//# sourceMappingURL=app.component.js.map