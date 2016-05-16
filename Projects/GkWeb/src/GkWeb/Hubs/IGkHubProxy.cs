using System;
using System.Threading.Tasks;
using RubezhAPI.GK;

namespace GkWeb.Hubs
{
	public interface IGkHubProxy
	{
		/// <summary>
		/// �������� ������� <paramref name="state"/> ��������� ��� ������� � UID = <paramref name="uid"/>.
		/// </summary>
		/// <param name="uid">UID �������</param>
		/// <param name="state">��������� �������</param>
		Task BroadcastEvent(Guid uid, GKState state);
	}
}