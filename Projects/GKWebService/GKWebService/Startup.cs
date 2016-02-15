#region Usings

using Microsoft.Owin;
using Owin;

#endregion

[assembly: OwinStartup(typeof (GKWebService.Startup))]

namespace GKWebService
{
	public class Startup
	{
		public void Configuration(IAppBuilder app) {
			// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
			app.MapSignalR();
		}
	}
}
