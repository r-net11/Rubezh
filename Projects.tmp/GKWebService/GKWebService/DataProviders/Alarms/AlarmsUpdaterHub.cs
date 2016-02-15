using GKWebService.Models;
using GKWebService.Models.GK.Alarms;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RubezhAPI.GK;

namespace GKWebService.DataProviders
{
	[HubName("alarmsUpdater")]
	public class AlarmsUpdaterHub : Hub
	{
		public static AlarmsUpdaterHub Instance { get; private set; }

		public AlarmsUpdaterHub()
		{
			Instance = this;
		}

		public void BroadcastAlarms()
		{
			var alarms = AlarmsViewModel.OnGKObjectsStateChanged(null);
			var alarmsViewModel = new AlarmsViewModel();
			alarmsViewModel.UpdateAlarms(alarms);
			var alarmGroupsViewModel = new AlarmGroupsViewModel();
			alarmGroupsViewModel.Update(alarms);
            Clients.All.updateAlarms(new {alarms = alarmsViewModel, groups = alarmGroupsViewModel});
		}
	}
}