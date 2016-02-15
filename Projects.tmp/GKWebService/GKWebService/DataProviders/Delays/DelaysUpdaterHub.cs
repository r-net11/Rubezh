using GKWebService.Models;
using Microsoft.AspNet.SignalR;

namespace GKWebService.DataProviders
{
	public class DelaysUpdaterHub : Hub
	{
		public static DelaysUpdaterHub Instance { get; private set; }
		public DelaysUpdaterHub()
		{
			Instance = this;
		}
		public void DelayUpdate(Delay delay)
		{
			Clients.All.delayUpdate(delay);
		}
	}
}