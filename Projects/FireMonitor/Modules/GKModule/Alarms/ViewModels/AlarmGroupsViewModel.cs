using System;
using System.Collections.Generic;
using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhClient;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class AlarmGroupsViewModel : BaseViewModel
	{
		public static AlarmGroupsViewModel Current { get; private set; }

		public AlarmGroupsViewModel()
		{
			Current = this;
			ResetCommand = new RelayCommand(OnReset, CanReset);
            GlobalPimActivationViewModel = new GlobalPimActivationViewModel();
            
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

		public List<AlarmGroupViewModel> AlarmGroups { get; private set; }
		public GlobalPimActivationViewModel GlobalPimActivationViewModel { get; private set; }

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