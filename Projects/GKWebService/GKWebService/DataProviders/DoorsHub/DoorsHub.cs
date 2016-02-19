using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;
using GKWebService.Models.Door;

namespace GKWebService.DataProviders.DoorsHub
{
	[HubName("DoorsUpdater")]
	public class DoorsHub : Hub
	{
		public static  DoorsHub Instance { get; private set; }
		public DoorsHub()
		{
			Instance = this;
		}
		public void DoorUpdate(GKDoor gkdoor)
		{
			Clients.All.doorUpdate(new Door(gkdoor));
		}
	}
}