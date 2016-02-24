using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GKWebService.DataProviders
{
	[HubName("configUpdater")]
	public class ConfigHub : Hub
	{
		public static ConfigHub Instance { get; private set; }
		public ConfigHub()
		{
			Instance = this;
		}
		public void ConfigHubUpdate()
		{
			Clients.All.configUpdate();
		}
	}
}