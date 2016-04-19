using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace GkWeb.Hubs
{
    public class TestHub : Hub
	{
		private readonly object _testBroadcastLock = new object();
		private readonly object _testSendMessageLock = new object();
		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
		private volatile bool _sendingTestMessage;
		private Timer _timer;

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

					Clients.All.broadcastMessage(message);

					_sendingTestMessage = false;
				}
			}
		}

	}
}
