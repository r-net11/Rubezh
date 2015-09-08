using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace GKWebService.DataProviders
{
	public class PlansUpdater
	{
		private readonly object _testBroadcastLock = new object();
		private readonly object _testSendMessageLock = new object();
		private Timer _timer;
		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
		private volatile bool _sendingTestMessage;


		// Singleton instance
		private readonly static Lazy<PlansUpdater> _instance = new Lazy<PlansUpdater>(
			() => new PlansUpdater(GlobalHost.ConnectionManager.GetHubContext<PlansUpdaterHub>().Clients));

		public static PlansUpdater Instance
		{
			get
			{
				return _instance.Value;
			}
		}

		private PlansUpdater(IHubConnectionContext<dynamic> clients)
		{
			Clients = clients;
			//LoadDefaultStocks();
		}

		private IHubConnectionContext<dynamic> Clients
		{
			get;
			set;
		}

		public void StartTestBroadcast()
		{
			lock (_testBroadcastLock)
			{
				_timer = new Timer(SendTestMessage, null, _updateInterval, _updateInterval);
			}
		}

		private void SendTestMessage(object state)
		{
			// This function must be re-entrant as it's running as a timer interval handler
			lock (_testSendMessageLock)
			{
				if (!_sendingTestMessage)
				{
					_sendingTestMessage = true;

					var message = "This is real-time test message with ID = " + new Random().Next();

					Clients.All.recieveTestMessage(message);

					_sendingTestMessage = false;
				}
			}
		}
	}
}