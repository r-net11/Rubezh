using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AlarmModule.ViewModels
{
	public class AlarmGroupsViewModel : BaseViewModel
	{
		public static AlarmGroupsViewModel Current { get; private set; }
		public AlarmGroupsViewModel()
		{
			Current = this;
			ResetCommand = new RelayCommand(OnReset);
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

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			AlarmsViewModel.Current.ResetAllCommand.Execute();
		}
	}
}