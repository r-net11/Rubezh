using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace AlarmModule.ViewModels
{
	public class AlarmGroupsViewModel : BaseViewModel
	{
        public static AlarmGroupsViewModel Current { get; private set; }
		public AlarmGroupsViewModel()
		{
            Current = this;
			AlarmGroups = new List<AlarmGroupViewModel>();
			foreach (AlarmType alarmType in Enum.GetValues(typeof(AlarmType)))
			{
				AlarmGroups.Add(new AlarmGroupViewModel() { AlarmType = alarmType });
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