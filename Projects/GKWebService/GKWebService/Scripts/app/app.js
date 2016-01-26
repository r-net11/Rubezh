(function() {
    'use strict';
    var app = angular.module('canvasApp', ['canvasApp.controllers', 'canvasApp.directives', 'canvasApp.services', 'ui.bootstrap']);
  
    angular.module('canvasApp.directives', []);
    angular.module('canvasApp.controllers', ['ui.grid']);
    angular.module('canvasApp.services', ['SignalR']);

}());