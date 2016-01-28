using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR.Hubs;
using GKWebService.Models;
using Microsoft.AspNet.SignalR;
using RubezhAPI.GK;
using RubezhAPI;
using Controls.Converters;
using GKWebService.Models.GK;
using GKModule.Converters;
using GKWebService.Utils;

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