#region Usings

using System;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

#endregion

namespace GKWebService.DataProviders.Plan
{
	public class PlansUpdater
	{
		// Singleton instance
		private static readonly Lazy<PlansUpdater> _instance = new Lazy<PlansUpdater>(
			() => new PlansUpdater(GlobalHost.ConnectionManager.GetHubContext<PlansUpdaterHub>().Clients));

		private readonly object _testBroadcastLock = new object();
		private readonly object _testSendMessageLock = new object();
		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
		private volatile bool _sendingTestMessage;
		private Timer _timer;

		private PlansUpdater(IHubConnectionContext<dynamic> clients) {
			Clients = clients;
		}

		private IHubConnectionContext<dynamic> Clients { get; set; }

		public static PlansUpdater Instance { get { return _instance.Value; } }

		public void StartTestBroadcast() {
			lock (_testBroadcastLock) {
				_timer = new Timer(SendTestMessage, null, _updateInterval, _updateInterval);
			}
		}

		private void SendTestMessage(object state) {
			// This function must be re-entrant as it's running as a timer interval handler
			lock (_testSendMessageLock) {
				if (!_sendingTestMessage) {
					_sendingTestMessage = true;

					var message = "This is real-time test message with ID = " + new Random().Next();

					Clients.All.recieveTestMessage(message);

					_sendingTestMessage = false;
				}
			}
		}

		public void UpdateDeviceState(object stateData) {
			Clients.All.updateDeviceState(stateData);
		}

        public void UpdateHint(object stateData) {
			Clients.All.updateHint(stateData);
		}
	}
}
