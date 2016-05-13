System.register(['@angular/core', '@angular/http', 'rxjs/add/operator/map', '../services/data.service'], function(exports_1, context_1) {
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
    var core_1, http_1, data_service_1;
    var UserIdentityComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (http_1_1) {
                http_1 = http_1_1;
            },
            function (_1) {},
            function (data_service_1_1) {
                data_service_1 = data_service_1_1;
            }],
        execute: function() {
            UserIdentityComponent = (function () {
                function UserIdentityComponent(dataService) {
                    this.dataService = dataService;
                }
                UserIdentityComponent.prototype.ngOnInit = function () { this.getUserName(); };
                UserIdentityComponent.prototype.getUserName = function () {
                    var _this = this;
                    this.dataService.getUserName().subscribe(function (res) { return _this.userName = res; });
                };
                UserIdentityComponent = __decorate([
                    core_1.Component({
                        selector: 'nav-identity',
                        templateUrl: 'app/shared/nav/user-identity.component.html',
                        providers: [http_1.HTTP_PROVIDERS, data_service_1.DataService]
                    }), 
                    __metadata('design:paramtypes', [data_service_1.DataService])
                ], UserIdentityComponent);
                return UserIdentityComponent;
            }());
            exports_1("UserIdentityComponent", UserIdentityComponent);
        }
    }
});
//# sourceMappingURL=user-identity.component.js.map