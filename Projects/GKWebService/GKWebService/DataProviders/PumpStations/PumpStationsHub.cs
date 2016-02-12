using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;
using GKWebService.Models.PumpStation;


namespace GKWebService.DataProviders.PumpStations
{
	[HubName("pumpStationsUpdater")]
	public class PumpStationsHub : Hub
	{
		public static PumpStationsHub Instance { get; set; }

		public PumpStationsHub()
		{
			Instance = this;
		}

		public void PumpStationstUpdate(GKPumpStation pumpStation)
		{ 
			var _pumpStation = new PumpStation(pumpStation);
			Clients.All.pumpStationstUpdate(_pumpStation);
		}
	}
}