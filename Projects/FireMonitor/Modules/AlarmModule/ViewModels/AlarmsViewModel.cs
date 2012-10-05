using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AlarmModule.Events;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace AlarmModule.ViewModels
{
	public class AlarmsViewModel : ViewPartViewModel
	{
		List<Alarm> allAlarms;
		AlarmType? _alarmType;

		public AlarmsViewModel()
		{
			ResetAllCommand = new RelayCommand(OnResetAll);
			RemoveAllFromIgnoreListCommand = new RelayCommand(OnRemoveAllFromIgnoreList, CanRemoveAllFromIgnoreList);
			ServiceFactory.Events.GetEvent<AlarmRemovedEvent>().Subscribe(OnAlarmRemoved);
			ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Subscribe(OnAlarmAdded);

			allAlarms = new List<Alarm>();
			Alarms = new ObservableCollection<AlarmViewModel>();
		}

		public void Initialize()
		{
		}

		public void Sort(AlarmType? alarmType)
		{
			_alarmType = alarmType;

			Alarms.Clear();

			foreach (var alarm in allAlarms)
			{
				if ((alarmType == null) || (alarm.AlarmType == alarmType))
				{
					var alarmViewModel = new AlarmViewModel(alarm);
					Alarms.Add(alarmViewModel);
				}
			}
		}

		public ObservableCollection<AlarmViewModel> Alarms { get; private set; }

		AlarmViewModel _selectedAlarm;
		public AlarmViewModel SelectedAlarm
		{
			get { return _selectedAlarm; }
			set
			{
				_selectedAlarm = value;
				OnPropertyChanged("SelectedAlarm");
			}
		}

		bool alrmWasReset;
		public bool HasAlarmsToReset
		{
			get
			{
				if (alrmWasReset)
					return false;
				return Alarms.Any(x => x.Alarm.AlarmType != AlarmType.Off);
			}
		}

		public RelayCommand ResetAllCommand { get; private set; }
		void OnResetAll()
		{
			var resetItems = new List<ResetItem>();
			foreach (var alarm in allAlarms)
			{
				var resetItem = alarm.GetResetItem();
				if (resetItem != null)
				{
					var existringResetItem = resetItems.FirstOrDefault(x => x.DeviceState == resetItem.DeviceState);
					if (existringResetItem != null)
					{
						foreach (var driverState in resetItem.States)
						{
							if (existringResetItem.States.Any(x => x.DriverState.Code == driverState.DriverState.Code) == false)
								existringResetItem.States.Add(driverState);
						}
					}
					else
					{
						resetItems.Add(resetItem);
					}
				}
			}

			alrmWasReset = true;
			OnPropertyChanged("HasAlarmsToReset");
            FiresecManager.FiresecDriver.ResetStates(resetItems);
		}

		public bool CanRemoveAllFromIgnoreList()
		{
			return Alarms.Any(x => x.Alarm.AlarmType == AlarmType.Off);
		}

		public RelayCommand RemoveAllFromIgnoreListCommand { get; private set; }
		void OnRemoveAllFromIgnoreList()
		{
			var devices = new List<Device>();
			foreach (var alarmViewModel in Alarms)
			{
				if (alarmViewModel.Alarm.AlarmType == AlarmType.Off)
				{
					var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == alarmViewModel.Alarm.DeviceUID);
					if (FiresecManager.CanDisable(device.DeviceState) && device.DeviceState.IsDisabled)
						devices.Add(device);
				}
			}

			if (ServiceFactory.SecurityService.Validate())
			{
                FiresecManager.FiresecDriver.RemoveFromIgnoreList(devices);
			}
		}

		void OnAlarmAdded(Alarm alarm)
		{
			allAlarms.Add(alarm);

			if (_alarmType == null || alarm.AlarmType == _alarmType)
			{
				Alarms.Insert(0, new AlarmViewModel(alarm));
				alrmWasReset = false;
				OnPropertyChanged("HasAlarmsToReset");
			}
		}

		void OnAlarmRemoved(Alarm alarm)
		{
			allAlarms.Remove(alarm);

			if (_alarmType == null || alarm.AlarmType == _alarmType)
			{
				Alarms.Remove(Alarms.FirstOrDefault(x => x.Alarm.DeviceUID == alarm.DeviceUID));
				OnPropertyChanged("HasAlarmsToReset");
			}
		}
	}
}