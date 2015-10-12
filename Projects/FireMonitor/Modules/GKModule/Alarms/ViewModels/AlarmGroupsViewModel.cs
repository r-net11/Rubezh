﻿using System;
using System.Collections.Generic;
using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class AlarmGroupsViewModel : BaseViewModel
	{
		public static AlarmGroupsViewModel Current { get; private set; }

		public AlarmGroupsViewModel()
		{
			Current = this;
			ResetCommand = new RelayCommand(OnReset, CanReset);
			AlarmGroups = new List<AlarmGroupViewModel>();
			foreach (GKAlarmType alarmType in Enum.GetValues(typeof(GKAlarmType)))
			{
				AlarmGroups.Add(new AlarmGroupViewModel(alarmType));
			}
		}

		public List<AlarmGroupViewModel> AlarmGroups { get; private set; }

		public void Update(List<Alarm> alarms)
		{
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

				alarmGroup.Alarms = alarmViewModels;
				alarmGroup.Update();
			}
			OnPropertyChanged(() => IsCanReset);
			OnPropertyChanged(() => Count);
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			AlarmsViewModel.Current.ResetAll();
		}

		public bool IsCanReset
		{
			get
			{
				return Count > 0;
			}
		}

		bool CanReset()
		{
			return IsCanReset;
		}

		public int Count
		{
			get { return AlarmsViewModel.Current.GetAlarmsToResetCount(); }
		}
	}
}