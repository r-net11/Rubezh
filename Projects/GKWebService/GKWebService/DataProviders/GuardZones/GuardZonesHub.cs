using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;
using GKWebService.Models.GuardZones;

namespace GKWebService.DataProviders
{
	[HubName("guardZonesUpdater")]
	public class GuardZonesHub : Hub
	{
		public static GuardZonesHub Instance { get; private set; }
		public GuardZonesHub()
		{
			Instance = this; 
		}

		public void GuardZoneUpdate(GKGuardZone guardZone)
		{
			var _guardZone = new GuardZone(guardZone);
			Clients.All.guardZoneUpdate(_guardZone); 
		}
	}
}