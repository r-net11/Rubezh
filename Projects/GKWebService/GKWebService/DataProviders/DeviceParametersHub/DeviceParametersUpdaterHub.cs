using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GKWebService.Models.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;

namespace GKWebService.DataProviders.DeviceParametersHub
{
	[HubName("DeviceParametersUpdater")]
	public class DeviceParametersUpdaterHub : Hub
	{
		public static DeviceParametersUpdaterHub Instance { get; private set; }
		public DeviceParametersUpdaterHub()
		{
			Instance = this;
		}
		public void DeviceParameterUpdate(GKDeviceMeasureParameters parameters)
		{
			Clients.All.deviceParameterUpdate(parameters);
		}
	}
}