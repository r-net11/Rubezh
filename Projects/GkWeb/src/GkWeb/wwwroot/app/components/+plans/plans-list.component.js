System.register(['@angular/core', '@angular/router', '../../shared/services/index'], function(exports_1, context_1) {
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
    var PlansListComponent;
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
            PlansListComponent = (function () {
                function PlansListComponent(router, dataService) {
                    this.router = router;
                    this.dataService = dataService;
                }
                PlansListComponent.prototype.load = function () {
                    var _this = this;
                    this.dataService.getPlansList().subscribe(function (plans) { return _this.plans = plans; }, function (error) { return _this.errorMessage = error; });
                };
                PlansListComponent.prototype.ngOnInit = function () {
                    if (this.isSubElement !== null && this.isSubElement === true) {
                        return;
                    }
                    else {
                        this.load();
                    }
                };
                __decorate([
                    core_1.Input(), 
                    __metadata('design:type', Array)
                ], PlansListComponent.prototype, "plans", void 0);
                __decorate([
                    core_1.Input(), 
                    __metadata('design:type', Boolean)
                ], PlansListComponent.prototype, "isSubElement", void 0);
                PlansListComponent = __decorate([
                    core_1.Component({
                        selector: '[gk-plans-list]',
                        templateUrl: 'app/components/+plans/plans-list.component.html',
                        styleUrls: ['app/components/+plans/plans-list.component.css'],
                        directives: [router_1.ROUTER_DIRECTIVES, PlansListComponent],
                        providers: [index_1.DataService]
                    }), 
                    __metadata('design:paramtypes', [router_1.Router, index_1.DataService])
                ], PlansListComponent);
                return PlansListComponent;
            }());
            exports_1("PlansListComponent", PlansListComponent);
        }
    }
});
//# sourceMappingURL=plans-list.component.js.map