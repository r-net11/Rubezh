System.register(['@angular/core', '@angular/router', './hello/hello-world.component'], function(exports_1, context_1) {
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
    var core_1, router_1, hello_world_component_1;
    var AppComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (router_1_1) {
                router_1 = router_1_1;
            },
            function (hello_world_component_1_1) {
                hello_world_component_1 = hello_world_component_1_1;
            }],
        execute: function() {
            AppComponent = (function () {
                function AppComponent(_router) {
                    this._router = _router;
                }
                AppComponent.prototype.clicked = function (event) {
                    event.preventDefault();
                    this._router.navigate(['/hello-world']);
                };
                AppComponent.prototype.ngOnInit = function () {
                    // init winjs menu
                    WinJS.UI.processAll().done(function () {
                        var splitView = document.querySelector(".splitView").winControl;
                    });
                };
                AppComponent = __decorate([
                    core_1.Component({
                        selector: 'gk-app',
                        templateUrl: 'views/layout/layout.html',
                        directives: [router_1.ROUTER_DIRECTIVES]
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
                    __metadata('design:paramtypes', [router_1.Router])
                ], AppComponent);
                return AppComponent;
            }());
            exports_1("AppComponent", AppComponent);
        }
    }
});
//# sourceMappingURL=app.component.js.map