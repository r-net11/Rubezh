using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GKWebService.DataProviders
{
    [HubName("plansrtstatusupdaterhub")]
    public class PlansRTStatusUpdaterHub : Hub
    {
        public void Test(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addTestMessage(name, message);
        }
    }
}