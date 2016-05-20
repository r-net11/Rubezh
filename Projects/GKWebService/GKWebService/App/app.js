(function () {
	'use strict';
	var app = angular.module('gkApp', [
		'ui.router',
		'gkApp.controllers',
		'gkApp.directives',
		'gkApp.services',
		'ui.bootstrap',
		'ui.grid',
		'ui.grid.treeView',
		'ui.grid.autoResize',
		'ui.grid.selection',
		'ui.tree',
		'ui.grid.cellNav',
        'ui.grid.saveState',
        'ui.grid.autoSizeColumn'
	]);

	angular.module('gkApp.directives', []);
	angular.module('gkApp.controllers', ['ui.grid', 'ui.grid.autoResize', 'ui.grid.selection', 'ui.grid.resizeColumns']);
	angular.module('gkApp.services', ['SignalR', 'ngCookies']);

	app.config(function ($httpProvider) {
	    $httpProvider.interceptors.push('authInterceptorService');
	});

	app.run(['authService', function (authService) {
	    authService.fillAuthData();
	}]);
}());

