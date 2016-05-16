using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using RubezhAPI.GK;

namespace GkWeb.Hubs
{
	[HubName("gkHub")]
	public class GkHub : Hub
	{
		private GkHubProxy _hubProxy;

		public GkHub(GkHubProxy hubProxy) {
			_hubProxy = hubProxy;
		}

		public override Task OnConnected() {
			return base.OnConnected();
		}


		public override Task OnDisconnected(bool stopCalled) {
			return base.OnDisconnected(stopCalled);
		}

		public override Task OnReconnected() {
			return base.OnReconnected();
		}

		/// <summary>
		/// Добавляет клиента в группу, по которой транслируются события для объекта с UID = <paramref name="elementUid"/>.
		/// </summary>
		/// <param name="elementUid">UID объекта</param>
		/// <returns></returns>
		public async Task SubscribeId(Guid elementUid) {
			await Groups.Add(Context.ConnectionId, elementUid.ToString());
		}

		/// <summary>
		/// Удаляет клиента из группы, по которой транслируются события для объекта с UID = <paramref name="elementUid"/>.
		/// </summary>
		/// <param name="elementUid">UID объекта</param>
		/// <returns></returns>
		public async Task UnsubscribeId(Guid elementUid) {
			await Groups.Remove(Context.ConnectionId, elementUid.ToString());
		}
	}
}
