using GKWebService.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;

namespace GKWebService.DataProviders.Devices
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