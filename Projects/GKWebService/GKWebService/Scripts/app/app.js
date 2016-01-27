(function() {
    'use strict';
    var app = angular.module('canvasApp', ['canvasApp.controllers', 'canvasApp.directives', 'canvasApp.services', 'ui.bootstrap', 'ui.grid', 'ui.grid.autoResize']);
  
    angular.module('canvasApp.directives', []);
    angular.module('canvasApp.controllers', ['ui.grid']);
    angular.module('canvasApp.services', ['SignalR']);

}());