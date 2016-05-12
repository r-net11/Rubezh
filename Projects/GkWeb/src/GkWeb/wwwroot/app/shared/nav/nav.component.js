System.register(['@angular/core', '@angular/router', '../../+components/index'], function(exports_1, context_1) {
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
    var core_1, router_1, index_1;
    var NavComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (router_1_1) {
                router_1 = router_1_1;
            },
            function (index_1_1) {
                index_1 = index_1_1;
            }],
        execute: function() {
            NavComponent = (function () {
                function NavComponent(_router) {
                    this._router = _router;
                }
                NavComponent.prototype.ngOnInit = function () {
                };
                NavComponent = __decorate([
                    core_1.Component({
                        selector: 'gk-nav',
                        templateUrl: 'app/shared/nav/nav.component.html',
                        directives: [router_1.ROUTER_DIRECTIVES, index_1.PlansComponent]
                    }),
                    router_1.Routes([
                        { path: '/plans', component: index_1.PlansComponent }
                    ]), 
                    __metadata('design:paramtypes', [router_1.Router])
                ], NavComponent);
                return NavComponent;
            }());
            exports_1("NavComponent", NavComponent);
        }
    }
});
//# sourceMappingURL=nav.component.js.map