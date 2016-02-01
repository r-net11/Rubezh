(function() {
    'use strict';
    var app = angular.module('gkApp', ['gkApp.controllers', 'gkApp.directives', 'gkApp.services', 'ui.bootstrap', 'ui.grid', 'ui.grid.treeView', 'ui.grid.autoResize', 'ui.grid.selection']);
  
    angular.module('gkApp.directives', []);
    angular.module('gkApp.controllers', ['ui.grid', 'ui.grid.autoResize', 'ui.grid.selection', 'ui.grid.resizeColumns']);
    angular.module('gkApp.services', ['SignalR']);

}());