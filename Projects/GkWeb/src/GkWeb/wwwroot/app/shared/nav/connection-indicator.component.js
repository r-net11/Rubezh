System.register(["@angular/core", "rxjs/add/operator/map", "../services/index"], function(exports_1, context_1) {
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
    var core_1, index_1;
    var ConnectionIndicatorComponent, ConnectionState;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (_1) {},
            function (index_1_1) {
                index_1 = index_1_1;
            }],
        execute: function() {
            ConnectionIndicatorComponent = (function () {
                function ConnectionIndicatorComponent(gkService) {
                    var _this = this;
                    this.gkService = gkService;
                    this.channel = "connection";
                    // Подписываемся на observables для SignalR
                    //
                    this.connectionState = new ConnectionState();
                    this.gkService.connectionState
                        .subscribe(function (state) {
                        console.info("Connection state has changed.");
                        _this.connectionState.fromState(state);
                    });
                    //this.gkService.error.subscribe(
                    //	(error: any) => {
                    //		console.warn(error);
                    //	},
                    //	(error: any) => {
                    //		console.error("errors$ error", error);
                    //	}
                    //);
                    //this.gkService.starting.subscribe(
                    //	() => {
                    //		console.log(
                    //			"signalr service has been started");
                    //	},
                    //	() => {
                    //		console.warn(
                    //			"signalr service failed to start!");
                    //	}
                    //);
                }
                ConnectionIndicatorComponent.prototype.ngOnInit = function () {
                };
                ConnectionIndicatorComponent.prototype.restartConnection = function () {
                    if (this.connectionState.Class === "disconnected") {
                        this.gkService.start();
                    }
                };
                ConnectionIndicatorComponent = __decorate([
                    core_1.Component({
                        selector: "[nav-connection]",
                        templateUrl: "app/shared/nav/connection-indicator.component.html",
                        styleUrls: ["app/shared/nav/connection-indicator.component.css"]
                    }), 
                    __metadata('design:paramtypes', [index_1.GkService])
                ], ConnectionIndicatorComponent);
                return ConnectionIndicatorComponent;
            }());
            exports_1("ConnectionIndicatorComponent", ConnectionIndicatorComponent);
            ConnectionState = (function () {
                function ConnectionState() {
                    this.Text = "Подключение отсутствует";
                    this.Class = "disconnected";
                }
                ConnectionState.prototype.fromState = function (state) {
                    switch (state) {
                        case 0 /* Connecting */ .valueOf():
                            {
                                this.Text = "Выполняется подключение к серверу";
                                this.Class = "connecting";
                                break;
                            }
                        case 1 /* Connected */ .valueOf():
                            {
                                this.Text = "Подключено к серверу";
                                this.Class = "connected";
                                break;
                            }
                        case 4 /* Disconnected */ .valueOf():
                            {
                                this.Text = "Подключение отсутствует";
                                this.Class = "disconnected";
                                break;
                            }
                        case 2 /* Reconnecting */ .valueOf():
                            {
                                this.Text = "Выполняется попытка переподключения к серверу";
                                this.Class = "reconnecting";
                                break;
                            }
                        default:
                            {
                                this.Text = "Подключение отсутствует";
                                this.Class = "disconnected";
                                break;
                            }
                    }
                };
                return ConnectionState;
            }());
        }
    }
});
//# sourceMappingURL=connection-indicator.component.js.map