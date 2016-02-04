using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;
using GKWebService.Models;

namespace GKWebService.DataProviders
{
	[HubName("devicesUpdater")]
	public class DevicesHub : Hub
	{
		public static DevicesHub Instance { get; private set; }
		public DevicesHub()
		{
			Instance = this;
		}
		 public void DevicesUpdate(GKDevice device)
		{
			var deviceModel = new Device (device);
			Clients.All.devicesUpdate(deviceModel);
		}
	}
}