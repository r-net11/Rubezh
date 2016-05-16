using System;
using System.Threading.Tasks;
using RubezhAPI.GK;

namespace GkWeb.Hubs
{
	public interface IGkHubProxy
	{
		/// <summary>
		/// Отсылает событие <paramref name="state"/> состояния для объекта с UID = <paramref name="uid"/>.
		/// </summary>
		/// <param name="uid">UID объекта</param>
		/// <param name="state">состояние объекта</param>
		Task BroadcastEvent(Guid uid, GKState state);
	}
}