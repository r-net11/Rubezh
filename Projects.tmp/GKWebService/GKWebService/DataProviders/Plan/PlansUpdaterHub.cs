#region Usings

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

#endregion

namespace GKWebService.DataProviders.Plan
{
	[HubName("plansUpdater")]
	public class PlansUpdaterHub : Hub
	{
		private readonly PlansUpdater _plansUpdater;

		public PlansUpdaterHub()
			: this(PlansUpdater.Instance) {
		}

		public PlansUpdaterHub(PlansUpdater plansUpdater) {
			_plansUpdater = plansUpdater;
		}

		public void StartTestBroadcast() {
			_plansUpdater.StartTestBroadcast();
		}
	}
}
