using GKWebService.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;

namespace GKWebService.DataProviders
{
	[HubName("directionsUpdater")]
	public class DirectionsUpdaterHub : Hub
	{
		public static DirectionsUpdaterHub Instance { get; private set; }

		public DirectionsUpdaterHub()
		{
			Instance = this;
		}

		public void BroadcastDirection(GKDirection gkDirection)
		{
			var direction = new Direction(gkDirection);
			Clients.All.updateDirection(direction);
		}
	}
}