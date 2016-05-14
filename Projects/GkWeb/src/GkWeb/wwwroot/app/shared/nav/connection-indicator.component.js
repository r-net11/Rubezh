System.register(['@angular/core', '@angular/http', 'rxjs/add/operator/map', '../services/gk.service'], function(exports_1, context_1) {
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
    var core_1, http_1, gk_service_1;
    var ConnectionIndicatorComponent;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (http_1_1) {
                http_1 = http_1_1;
            },
            function (_1) {},
            function (gk_service_1_1) {
                gk_service_1 = gk_service_1_1;
            }],
        execute: function() {
            ConnectionIndicatorComponent = (function () {
                function ConnectionIndicatorComponent(gkService, http) {
                    this.gkService = gkService;
                    this.http = http;
                    this.channel = "connection";
                    // Let's wire up to the signalr observables
                    //
                    this.connectionState$ = this.gkService.connectionState$
                        .map(function (state) { return gk_service_1.ConnectionState[state]; });
                    this.gkService.error$.subscribe(function (error) { console.warn(error); }, function (error) { console.error("errors$ error", error); });
                    // Wire up a handler for the starting$ observable to log the
                    //  success/fail result
                    //
                    this.gkService.starting$.subscribe(function () { console.log("signalr service has been started"); }, function () { console.warn("signalr service failed to start!"); });
                }
                ConnectionIndicatorComponent.prototype.ngOnInit = function () {
                    // Start the connection up!
                    //
                    console.log("Starting the channel service");
                    this.gkService.start();
                };
                ConnectionIndicatorComponent = __decorate([
                    core_1.Component({
                        selector: 'nav-connection',
                        templateUrl: 'app/shared/nav/connection-indicator.component.html',
                        providers: [gk_service_1.GkService]
                    }), 
                    __metadata('design:paramtypes', [gk_service_1.GkService, http_1.Http])
                ], ConnectionIndicatorComponent);
                return ConnectionIndicatorComponent;
            }());
            exports_1("ConnectionIndicatorComponent", ConnectionIndicatorComponent);
        }
    }
});
//# sourceMappingURL=connection-indicator.component.js.map