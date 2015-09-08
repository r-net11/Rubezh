using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GKWebService.DataProviders
{
    [HubName("plansUpdater")]
    public class PlansUpdaterHub : Hub
    {

		private readonly PlansUpdater _plansUpdater;
		public PlansUpdaterHub() :
            this(PlansUpdater.Instance)
        {

		}

		public PlansUpdaterHub(PlansUpdater plansUpdater)
		{
			_plansUpdater = plansUpdater;
		}

		public void StartTestBroadcast()
        {
			_plansUpdater.StartTestBroadcast();
        }
    }
}