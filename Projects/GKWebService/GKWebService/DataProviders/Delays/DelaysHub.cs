using Microsoft.AspNet.SignalR;

namespace GKWebService.DataProviders.Delays
{
	public class DelaysHub : Hub
	{
		public void DelaysUpdate(string name, string message)
		{
			Clients.All.addNewMessageToPage(name, message);
		}
	}
}