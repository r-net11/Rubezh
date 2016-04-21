/// <binding AfterBuild='copyLibs, copyLibs:d3' Clean='clean:libs' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify");
var bower = require('gulp-bower');


var paths = {
	webroot: "./wwwroot/"
};
paths.concatJsDest = paths.webroot + "lib";

gulp.task("clean:libs", function (cb) {
	rimraf(paths.concatJsDest, cb);
});

gulp.task('bower', function() {
  return bower().pipe(gulp.dest('./wwwroot/lib/'));;
});

gulp.task("copyLibs:angular", function () {
	gulp.src(['./node_modules/angular2/bundles/*.*', './node_modules/systemjs/dist/*.*', './node_modules/es6-shim/*.js', './node_modules/es6-shim/*.map', './node_modules/angular2/es6/dev/src/testing/shims_for_IE.js', './node_modules/rxjs/bundles/*.*'])
	.pipe(gulp.dest('./wwwroot/lib/angular2/'));
});
gulp.task("copyLibs:d3", function () {
	gulp.src(['./node_modules/d3/d3*.js'])
	.pipe(gulp.dest('./wwwroot/lib/d3/'));
});

gulp.task("copyLibs:winjs", function () {
	gulp.src(['./node_modules/winjs/js/*.js', './node_modules/winjs/css/*.css', './node_modules/winjs/fonts/*.ttf'])
	.pipe(gulp.dest('./wwwroot/lib/winjs/'));
});

gulp.task("copyLibs", ["copyLibs:angular", "copyLibs:d3", "copyLibs:winjs"]);

//paths.js = paths.webroot + "js/**/*.js";
//paths.minJs = paths.webroot + "js/**/*.min.js";
//paths.css = paths.webroot + "css/**/*.css";
//paths.minCss = paths.webroot + "css/**/*.min.css";

//paths.concatCssDest = paths.webroot + "css/site.min.css";



//gulp.task("clean:css", function (cb) {
//    rimraf(paths.concatCssDest, cb);
//});

//gulp.task("clean", ["clean:js", "clean:css"]);

//gulp.task("min:js", function () {
//    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
//        .pipe(concat(paths.concatJsDest))
//        .pipe(uglify())
//        .pipe(gulp.dest("."));
//});

//gulp.task("min:css", function () {
//    return gulp.src([paths.css, "!" + paths.minCss])
//        .pipe(concat(paths.concatCssDest))
//        .pipe(cssmin())
//        .pipe(gulp.dest("."));
//});

//gulp.task("min", ["min:js", "min:css"]);
