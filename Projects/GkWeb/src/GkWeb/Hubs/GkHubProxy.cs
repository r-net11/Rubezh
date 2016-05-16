using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Extensions.Logging;
using RubezhAPI.GK;

namespace GkWeb.Hubs
{
	public class GkHubProxy : IGkHubProxy
	{
		private readonly IHubContext<GkHub> _hubContext;
		private ILogger<GkHubProxy> _logger;

		private GkHubProxy(IHubContext<GkHub> hubContext, ILogger<GkHubProxy> logger) {
			_hubContext = hubContext;
			_logger = logger;
		}

		/// <summary>
		/// Отсылает событие <paramref name="state"/> состояния для объекта с UID = <paramref name="uid"/>.
		/// </summary>
		/// <param name="uid">UID объекта</param>
		/// <param name="state">состояние объекта</param>
		public async Task BroadcastEvent(Guid uid, GKState state) {
			await _hubContext.Clients.Group(uid.ToString()).send(state);
		}
	}
}
