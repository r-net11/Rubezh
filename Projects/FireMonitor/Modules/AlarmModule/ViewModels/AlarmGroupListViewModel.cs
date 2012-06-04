using System;
using System.Collections.Generic;
using AlarmModule.Events;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace AlarmModule.ViewModels
{
	public class AlarmGroupListViewModel : ViewPartViewModel
	{
		public AlarmGroupListViewModel()
		{
			AlarmGroups = new List<AlarmGroupViewModel>();
			foreach (AlarmType alarmType in Enum.GetValues(typeof(AlarmType)))
			{
				AlarmGroups.Add(new AlarmGroupViewModel() { AlarmType = alarmType });
			}
		}

		public List<AlarmGroupViewModel> AlarmGroups { get; private set; }
	}
}