using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace GKWebService
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

			//bundles.Add(new ScriptBundle("~/bundles/libs").Include(
			//	"~/Scripts/angular.js",
			//	"~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
			//	"~/Scripts/d3/d3.min.js",
			//	"~/Scripts/d3tip.js"));

			//bundles.Add(new ScriptBundle("~/bundles/shapesApp").Include(
			//		  "~/Scripts/app/app.js",
			//		  "~/Scripts/app/controllers/planCtrl.js",
			//		  "~/Scripts/app/controllers/signalrCtrl.js",
			//		  "~/Scripts/app/directives/d3Plan.js",
			//		  "~/Scripts/app/services/signalrService.js"));

			//bundles.Add(new ScriptBundle("~/bundles/fireZones").Include(
			//		  "~/Scripts/app/controllers/firezonesCtrl.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}