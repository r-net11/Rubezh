using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GKWebService.DataProviders.FireZones
{
    [HubName("fireZonesUpdater")]
    public class FireZonesUpdaterHub : Hub
    {
        private readonly FireZonesUpdater _fireZonesUpdater;

        public FireZonesUpdaterHub()
            : this(FireZonesUpdater.Instance)
        {
        }

        public FireZonesUpdaterHub(FireZonesUpdater fireZonesUpdater){
            _fireZonesUpdater = fireZonesUpdater;
		}
        
        public void StartStatesMonitoring(){
            _fireZonesUpdater.StartStatesMonitoring();
        }
    }
}