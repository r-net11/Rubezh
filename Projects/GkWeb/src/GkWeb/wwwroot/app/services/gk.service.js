System.register(["@angular/core", 'rxjs/Subject', 'rxjs/add/operator/map'], function(exports_1, context_1) {
    "use strict";
    var __moduleName = context_1 && context_1.id;
    var __extends = (this && this.__extends) || function (d, b) {
        for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
    var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
        var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
        if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
        else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
        return c > 3 && r && Object.defineProperty(target, key, r), r;
    };
    var __metadata = (this && this.__metadata) || function (k, v) {
        if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
    };
    var __param = (this && this.__param) || function (paramIndex, decorator) {
        return function (target, key) { decorator(target, key, paramIndex); }
    };
    var core_1, Subject_1;
    var SignalrWindow, ConnectionState, ChannelConfig, ChannelEvent, ChannelSubject, GkService;
    return {
        setters:[
            function (core_1_1) {
                core_1 = core_1_1;
            },
            function (Subject_1_1) {
                Subject_1 = Subject_1_1;
            },
            function (_1) {}],
        execute: function() {
            SignalrWindow = (function (_super) {
                __extends(SignalrWindow, _super);
                function SignalrWindow() {
                    _super.apply(this, arguments);
                }
                return SignalrWindow;
            }(Window));
            exports_1("SignalrWindow", SignalrWindow);
            (function (ConnectionState) {
                ConnectionState[ConnectionState["Connecting"] = 1] = "Connecting";
                ConnectionState[ConnectionState["Connected"] = 2] = "Connected";
                ConnectionState[ConnectionState["Reconnecting"] = 3] = "Reconnecting";
                ConnectionState[ConnectionState["Disconnected"] = 4] = "Disconnected";
            })(ConnectionState || (ConnectionState = {}));
            exports_1("ConnectionState", ConnectionState);
            ChannelConfig = (function () {
                function ChannelConfig() {
                }
                return ChannelConfig;
            }());
            exports_1("ChannelConfig", ChannelConfig);
            ChannelEvent = (function () {
                function ChannelEvent() {
                    this.Timestamp = new Date();
                }
                return ChannelEvent;
            }());
            exports_1("ChannelEvent", ChannelEvent);
            ChannelSubject = (function () {
                function ChannelSubject() {
                }
                return ChannelSubject;
            }());
            /**
             * GkService is a wrapper around the functionality that SignalR
             * provides to expose the ideas of channels and events. With this service
             * you can subscribe to specific channels (or groups in signalr speak) and
             * use observables to react to specific events sent out on those channels.
             */
            GkService = (function () {
                function GkService(window, channelConfig) {
                    var _this = this;
                    this.window = window;
                    this.channelConfig = channelConfig;
                    // These are used to feed the public observables 
                    //
                    this.connectionStateSubject = new Subject_1.Subject();
                    this.startingSubject = new Subject_1.Subject();
                    this.errorSubject = new Subject_1.Subject();
                    // An internal array to track what channel subscriptions exist 
                    //
                    this.subjects = new Array();
                    if (this.window.$ === undefined || this.window.$.hubConnection === undefined) {
                        throw new Error("The variable '$' or the .hubConnection() function are not defined...please check the SignalR scripts have been loaded properly");
                    }
                    // Set up our observables
                    //
                    this.connectionState$ = this.connectionStateSubject.asObservable();
                    this.error$ = this.errorSubject.asObservable();
                    this.starting$ = this.startingSubject.asObservable();
                    this.hubConnection = this.window.$.hubConnection();
                    this.hubConnection.url = channelConfig.url;
                    this.hubProxy = this.hubConnection.createHubProxy(channelConfig.hubName);
                    // Define handlers for the connection state events
                    //
                    this.hubConnection.stateChanged(function (state) {
                        var newState = ConnectionState.Connecting;
                        switch (state.newState) {
                            case _this.window.$.signalR.connectionState.connecting:
                                newState = ConnectionState.Connecting;
                                break;
                            case _this.window.$.signalR.connectionState.connected:
                                newState = ConnectionState.Connected;
                                break;
                            case _this.window.$.signalR.connectionState.reconnecting:
                                newState = ConnectionState.Reconnecting;
                                break;
                            case _this.window.$.signalR.connectionState.disconnected:
                                newState = ConnectionState.Disconnected;
                                break;
                        }
                        // Push the new state on our subject
                        //
                        _this.connectionStateSubject.next(newState);
                    });
                    // Define handlers for any errors
                    //
                    this.hubConnection.error(function (error) {
                        // Push the error on our subject
                        //
                        _this.errorSubject.next(error);
                    });
                    this.hubProxy.on("onEvent", function (channel, ev) {
                        //console.log(`onEvent - ${channel} channel`, ev);
                        // This method acts like a broker for incoming messages. We 
                        //  check the interal array of subjects to see if one exists
                        //  for the channel this came in on, and then emit the event
                        //  on it. Otherwise we ignore the message.
                        //
                        var channelSub = _this.subjects.find(function (x) {
                            return x.channel === channel;
                        });
                        // If we found a subject then emit the event on it
                        //
                        if (channelSub !== undefined) {
                            channelSub.subject.next(ev);
                        }
                    });
                }
                /**
                 * Start the SignalR connection. The starting$ stream will emit an
                 * event if the connection is established, otherwise it will emit an
                 * error.
                 */
                GkService.prototype.start = function () {
                    var _this = this;
                    // Now we only want the connection started once, so we have a special
                    //  starting$ observable that clients can subscribe to know know if
                    //  if the startup sequence is done.
                    //
                    // If we just mapped the start() promise to an observable, then any time
                    //  a client subscried to it the start sequence would be triggered
                    //  again since it's a cold observable.
                    //
                    this.hubConnection.start()
                        .done(function () {
                        _this.startingSubject.next(null);
                    })
                        .fail(function (error) {
                        _this.startingSubject.error(error);
                    });
                };
                /**
                 * Get an observable that will contain the data associated with a specific
                 * channel
                 * */
                GkService.prototype.sub = function (channel) {
                    var _this = this;
                    // Try to find an observable that we already created for the requested 
                    //  channel
                    //
                    var channelSub = this.subjects.find(function (x) {
                        return x.channel === channel;
                    });
                    // If we already have one for this event, then just return it
                    //
                    if (channelSub !== undefined) {
                        console.log("Found existing observable for " + channel + " channel");
                        return channelSub.subject.asObservable();
                    }
                    //
                    // If we're here then we don't already have the observable to provide the
                    //  caller, so we need to call the server method to join the channel 
                    //  and then create an observable that the caller can use to received
                    //  messages.
                    //
                    // Now we just create our internal object so we can track this subject
                    //  in case someone else wants it too
                    //
                    channelSub = new ChannelSubject();
                    channelSub.channel = channel;
                    channelSub.subject = new Subject_1.Subject();
                    this.subjects.push(channelSub);
                    // Now SignalR is asynchronous, so we need to ensure the connection is
                    //  established before we call any server methods. So we'll subscribe to 
                    //  the starting$ stream since that won't emit a value until the connection
                    //  is ready
                    //
                    this.starting$.subscribe(function () {
                        _this.hubProxy.invoke("Subscribe", channel)
                            .done(function () {
                            console.log("Successfully subscribed to " + channel + " channel");
                        })
                            .fail(function (error) {
                            channelSub.subject.error(error);
                        });
                    }, function (error) {
                        channelSub.subject.error(error);
                    });
                    return channelSub.subject.asObservable();
                };
                /** publish provides a way for calles to emit events on any channel. In a
                 * production app the server would ensure that only authorized clients can
                 * actually emit the message, but here we're not concerned about that.
                 */
                GkService.prototype.publish = function (ev) {
                    this.hubProxy.invoke("Publish", ev);
                };
                GkService = __decorate([
                    core_1.Injectable(),
                    __param(0, core_1.Inject(SignalrWindow)),
                    __param(1, core_1.Inject("channel.config")), 
                    __metadata('design:paramtypes', [SignalrWindow, ChannelConfig])
                ], GkService);
                return GkService;
            }());
            exports_1("GkService", GkService);
        }
    }
});
//# sourceMappingURL=gk.service.js.map