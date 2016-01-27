using Microsoft.AspNet.SignalR;

namespace GKWebService.DataProviders
{
	public class DelaysHub : Hub
	{
		public static DelaysHub Instance { get; private set; }
		public void CreateInstance()
		{
			Instance = this;
		}
		public void DelayStateIconUpdate(string delayUid, string stateIcon)
		{
			Clients.All.delayStateUpdate(delayUid, stateIcon);
		}
	}
}