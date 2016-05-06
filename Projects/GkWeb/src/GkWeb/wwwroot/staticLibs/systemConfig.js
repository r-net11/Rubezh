(function (global) {

	// map tells the System loader where to look for things
	var map = {
		'app': 'app', // 'dist',
		'rxjs': 'lib/rxjs',
		'@angular': 'lib/@angular'
	};

	// packages tells the System loader how to load when no filename and/or no extension
	var packages = {
		'app': { main: 'main.js', defaultExtension: 'js' },
		'rxjs': { defaultExtension: 'js' },
	};

	var packageNames = [
	  'lib/@angular/common',
	  'lib/@angular/compiler',
	  'lib/@angular/core',
	  'lib/@angular/http',
	  'lib/@angular/platform-browser',
	  'lib/@angular/platform-browser-dynamic',
	  'lib/@angular/router'
	];

	// add package entries for angular packages in the form '@angular/common': { main: 'index.js', defaultExtension: 'js' }
	packageNames.forEach(function (pkgName) {
		packages[pkgName] = { main: 'index.js', defaultExtension: 'js' };
	});

	var config = {
		map: map,
		packages: packages
	}

	// filterSystemConfig - index.html's chance to modify config before we register it.
	if (global.filterSystemConfig) { global.filterSystemConfig(config); }

	System.config(config);

})(this);