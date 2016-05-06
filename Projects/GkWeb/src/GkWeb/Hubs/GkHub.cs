using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace GkWeb.Hubs
{
    public class GkHub : Hub
	{
		private readonly object _testBroadcastLock = new object();
		private readonly object _testSendMessageLock = new object();
		private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
		private volatile bool _sendingTestMessage;
		private Timer _timer;

		public override Task OnConnected() {
			return base.OnConnected();
		}


		public override Task OnDisconnected(bool stopCalled) {
			return base.OnDisconnected(stopCalled);
		}

		/// <summary>
		/// Добавляет клиента в группу, по которой транслируются события для объекта с UID = <paramref name="elementUid"/>.
		/// </summary>
		/// <param name="elementUid">UID объекта</param>
		/// <returns></returns>
	    public Task SubscribeId(Guid elementUid) {
			return Groups.Add(Context.ConnectionId, elementUid.ToString());
		}

		/// <summary>
		/// Удаляет клиента из группы, по которой транслируются события для объекта с UID = <paramref name="elementUid"/>.
		/// </summary>
		/// <param name="elementUid">UID объекта</param>
		/// <returns></returns>
		public Task UnsubscribeId(Guid elementUid) {
			return Groups.Remove(Context.ConnectionId, elementUid.ToString());
		}

		/// <summary>
		/// Отсылает событие <paramref name="state"/> состояния для объекта с UID = <paramref name="uid"/>.
		/// </summary>
		/// <param name="uid">UID объекта</param>
		/// <param name="state">состояние объекта</param>
		private void BroadcastEvent(Guid uid, object state) {
			Clients.Group(uid.ToString()).send(state);
		}
	}
}
