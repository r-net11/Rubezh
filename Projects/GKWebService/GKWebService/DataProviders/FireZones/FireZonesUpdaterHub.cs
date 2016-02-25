using GKWebService.Models.FireZone;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;

namespace GKWebService.DataProviders.FireZones
{
    [HubName("fireZonesUpdater")]
    public class FireZonesUpdaterHub : Hub
    {
        public static FireZonesUpdaterHub Instance { get; private set; }

        public FireZonesUpdaterHub()
		{
			Instance = this;
		}

		public void BroadcastFireZone(GKZone gkZone)
		{
			Clients.All.updateFireZone(new FireZone(gkZone));
		}

    }
}