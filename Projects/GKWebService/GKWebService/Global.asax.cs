using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GKWebService.DataProviders;
using GKWebService.DataProviders.Plan;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using Owin;

namespace GKWebService
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start() {
			GlobalConfiguration.Configure(WebApiConfig.Register);
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			Thread bootstrapperThread = new Thread(Bootstrapper.Run) {IsBackground = true};
			bootstrapperThread.Start();
			Debug.WriteLine(string.Format("App thread is {0}, with appartment = {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.GetApartmentState().ToString()));

			Application["bootstrapperThread"] = bootstrapperThread;

			//Bootstrapper.Run();
			PlansDataProvider.Instance.GetInfo();
		}

		protected void Application_End() {
			try {
				Thread bootstrapperThread = (Thread)Application["bootstrapperThread"];
				if (bootstrapperThread != null && bootstrapperThread.IsAlive) {
					bootstrapperThread.Abort();
				}
			}
			catch {
				///
			}
		}
	}


}
