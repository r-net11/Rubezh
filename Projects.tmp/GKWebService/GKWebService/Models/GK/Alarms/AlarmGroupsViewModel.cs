using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKWebService.Models.GK.Alarms
{
	public class AlarmGroupsViewModel
	{
		public List<AlarmGroupViewModel> AlarmGroups { get; private set; }

		public bool CanReset { get; set; }

		public int Count { get; set; }

		public AlarmGroupsViewModel()
		{
			AlarmGroups = new List<AlarmGroupViewModel>();
			if (GKManager.Directions.Count > 0 || GKManager.MPTs.Count > 0)
				AlarmGroups.Add(new AlarmGroupViewModel(GKAlarmType.NPTOn));
			if (GKManager.Doors.Count > 0 || GKManager.GuardZones.Count > 0)
				AlarmGroups.Add(new AlarmGroupViewModel(GKAlarmType.GuardAlarm));
			if (GKManager.Zones.Count > 0)
			{
				AlarmGroups.Add(new AlarmGroupViewModel(GKAlarmType.Fire2));
				AlarmGroups.Add(new AlarmGroupViewModel(GKAlarmType.Fire1));
			}
			AlarmGroups.Add(new AlarmGroupViewModel(GKAlarmType.Attention));
			AlarmGroups.Add(new AlarmGroupViewModel(GKAlarmType.Failure));
			AlarmGroups.Add(new AlarmGroupViewModel(GKAlarmType.Ignore));
			AlarmGroups.Add(new AlarmGroupViewModel(GKAlarmType.AutoOff));
			AlarmGroups.Add(new AlarmGroupViewModel(GKAlarmType.Service));
			AlarmGroups.Add(new AlarmGroupViewModel(GKAlarmType.Turning));
		}

		public void Update(List<Alarm> alarms)
		{
			Count = AlarmsViewModel.GetAlarmsToResetCount();

			CanReset = (Count > 0);

			foreach (var alarmGroup in AlarmGroups)
			{
				var alarmViewModels = new List<AlarmViewModel>();
				foreach (var alarm in alarms)
				{
					if (alarm.AlarmType == alarmGroup.AlarmType)
					{
						var alarmViewModel = new AlarmViewModel(alarm);
						alarmViewModels.Add(alarmViewModel);
					}
				}

				alarmGroup.Update(alarmViewModels);
			}
		}
	}
}