#region Usings

using System.Diagnostics;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GKWebService.DataProviders.Plan;

#endregion

namespace GKWebService
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start() {
			GlobalConfiguration.Configure(WebApiConfig.Register);
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			var bootstrapperThread = new Thread(Bootstrapper.Run) { IsBackground = true };
			bootstrapperThread.Start();

			Application["bootstrapperThread"] = bootstrapperThread;
		}

		protected void Application_End() {
			try {
				var bootstrapperThread = (Thread)Application["bootstrapperThread"];
				if (bootstrapperThread != null
				    && bootstrapperThread.IsAlive) {
					bootstrapperThread.Abort();
				}
			}
			catch {
				///
			}
		}

		protected void Application_BeginRequest()
		{
			Context.Response.SuppressFormsAuthenticationRedirect = true;
		}
	}
}
