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
	[HubName("mptHub")]
	public class MPTHub : Hub
	{
		public static MPTHub Instance { get; private set; }
		 MPTHub()
		{
			Instance = this;
		}
		 public void MPTStateIconUpdate(GKMPT mpt)
		{
			var mptModel = new MPTModel(mpt);
			Clients.All.mptStateUpdate(mpt);
		}
	}
}