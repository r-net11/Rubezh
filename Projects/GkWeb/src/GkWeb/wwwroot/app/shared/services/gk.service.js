System.register(["@angular/core", 'rxjs/Subject'], function(exports_1, context_1) {
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
    var core_1, Subject_1;
    var GkService;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (Subject_1_1) {
                Subject_1 = Subject_1_1;
            }],
        execute: function() {
            GkService = (function () {
                function GkService() {
                    var _this = this;
                    // Эти поля используются для трансляции в соответствующие публичные Observable
                    //
                    this.connectionStateSubject = new Subject_1.Subject();
                    this.startingSubject = new Subject_1.Subject();
                    this.errorSubject = new Subject_1.Subject();
                    // Настройка observables
                    //
                    this.connectionState = this.connectionStateSubject.asObservable();
                    this.error = this.errorSubject.asObservable();
                    this.starting = this.startingSubject.asObservable();
                    // Создаем JavaScript-прокси для хаба
                    //
                    this.hubConnection = $.connection.hub;
                    //this.hubProxy = this.hubConnection.createHubProxy("gkHub");
                    // Задаем обработчики событий подключения
                    //
                    this.hubConnection.stateChanged(function (change) {
                        if (change.newState === change.oldState) {
                            return;
                        }
                        console.info("Signalr connection state changed.");
                        // Проталкиваем новое состояние в субъект
                        //
                        _this.connectionStateSubject.next(change.newState);
                    });
                    //this.hubConnection.error(this.onConnectionError);
                    this.hubConnection.connectionSlow(function () { console.info("Signalr connection is slow."); });
                    this.hubConnection.disconnected(function () { console.info("Signalr disconnected."); });
                    //this.hubConnection.error((e) => { console.error("Signalr connection error." + e.message) });
                    this.hubConnection.reconnected(function () { console.info("Signalr connection reconnected."); });
                    this.hubConnection.reconnecting(function () { console.info("Signalr connection reconnecting."); });
                }
                GkService.prototype.onConnectionError = function (error) {
                    // Проталкиваем сообщение в субъект
                    //
                    this.errorSubject.next(error.message);
                };
                GkService.prototype.onStateChanged = function (change) {
                    if (change.newState === change.oldState) {
                        return;
                    }
                    // Проталкиваем новое состояние в субъект
                    //
                    //this.connectionStateSubject.next(change.newState);
                };
                GkService.prototype.start = function () {
                    this.hubConnection.start().done(function () {
                        //this.startingSubject.next(null);
                    })
                        .fail(function (error) {
                        //this.startingSubject.error(error);
                    });
                };
                GkService = __decorate([
                    core_1.Injectable(), 
                    __metadata('design:paramtypes', [])
                ], GkService);
                return GkService;
            }());
            exports_1("GkService", GkService);
        }
    }
});
//# sourceMappingURL=gk.service.js.map