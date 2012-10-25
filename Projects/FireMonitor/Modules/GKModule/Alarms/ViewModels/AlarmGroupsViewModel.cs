using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class AlarmsGroupsViewModel : BaseViewModel
	{
		public static AlarmsGroupsViewModel Current { get; private set; }

		public AlarmsGroupsViewModel()
		{
			Current = this;
			AlarmGroups = new List<AlarmGroupViewModel>();
			foreach (XAlarmType alarmType in Enum.GetValues(typeof(XAlarmType)))
			{
				AlarmGroups.Add(new AlarmGroupViewModel(alarmType));
			}
		}

		public List<AlarmGroupViewModel> AlarmGroups { get; private set; }

		public void Update(List<Alarm> alarms)
		{
			foreach (var alarmGroup in AlarmGroups)
			{
				alarmGroup.Alarms = alarms.Where(x => x.AlarmType == alarmGroup.AlarmType).ToList<Alarm>();
				alarmGroup.Update();
			}
		}
	}
}