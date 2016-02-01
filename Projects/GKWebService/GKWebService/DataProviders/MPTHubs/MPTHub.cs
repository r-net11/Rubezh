using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using GKWebService.Models;
using RubezhAPI.GK;

namespace GKWebService.DataProviders.MPTHubs
{
	[HubName("mptsUpdater")]
	public class MptUpdaterHub : Hub
	{
		public static MptUpdaterHub Instance { get; private set; }
		public MptUpdaterHub()
		{
			Instance = this;
		}
		 public void MptUpdate(GKMPT mpt)
		{
			var mptModel = new MPTModel(mpt);
			Clients.All.mptUpdate(mptModel);
		}
	}
}