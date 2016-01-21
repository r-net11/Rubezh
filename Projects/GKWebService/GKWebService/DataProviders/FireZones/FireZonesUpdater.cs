using System;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GKWebService.DataProviders.FireZones
{
    public class FireZonesUpdater
    {
        // Singleton instance
		private static readonly Lazy<FireZonesUpdater> _instance = new Lazy<FireZonesUpdater>(
            () => new FireZonesUpdater(GlobalHost.ConnectionManager.GetHubContext<FireZonesUpdaterHub>().Clients));

		private readonly object _testBroadcastLock = new object();
		private readonly object _testSendMessageLock = new object();
		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
		private volatile bool _sendingTestMessage;
		private Timer _timer;

        private FireZonesUpdater(IHubConnectionContext<dynamic> clients)
        {
			Clients = clients;
		}

		private IHubConnectionContext<dynamic> Clients { get; set; }

        public static FireZonesUpdater Instance { get { return _instance.Value; } }

		public void StartTestBroadcast() {
			lock (_testBroadcastLock) {
                _timer = new Timer(TestMethod, "MessageText", _updateInterval, _updateInterval);
			}
		}


        private void TestMethod(object param)
        {
            Clients.All.testMethodOfAlex(param);
        }

    }
}
