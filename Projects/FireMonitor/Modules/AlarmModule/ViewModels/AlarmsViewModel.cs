using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Threading;
using System.Windows.Threading;

namespace AlarmModule.ViewModels
{
	public class AlarmsViewModel : ViewPartViewModel
	{
        public static AlarmsViewModel Current { get; private set; }
		List<Alarm> allAlarms;
		AlarmType? SortingAlarmType;

		public AlarmsViewModel()
		{
            Current = this;
			ResetAllCommand = new RelayCommand(OnResetAll, CanResetAll);
			RemoveAllFromIgnoreListCommand = new RelayCommand(OnRemoveAllFromIgnoreList, CanRemoveAllFromIgnoreList);

			allAlarms = new List<Alarm>();
			Alarms = new ObservableCollection<AlarmViewModel>();
		}

        public void Update(List<Alarm> alarms)
        {
            allAlarms = alarms;
            Sort(SortingAlarmType);
        }

		public void Sort(AlarmType? sortingAlarmType)
		{
			SortingAlarmType = sortingAlarmType;

            Alarm oldAlarm = null;
            if (SelectedAlarm != null)
            {
                oldAlarm = SelectedAlarm.Alarm.Clone();
            }

			Alarms.Clear();
			foreach (var alarm in allAlarms)
			{
				if ((sortingAlarmType == null) || (alarm.AlarmType == sortingAlarmType))
				{
					var alarmViewModel = new AlarmViewModel(alarm);
					Alarms.Add(alarmViewModel);
				}
			}

            if (oldAlarm != null)
            {
                SelectedAlarm = Alarms.FirstOrDefault(x => x.Alarm.IsEqualTo(oldAlarm));
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

			FiresecManager.FiresecDriver.ResetStates(resetItems);
			AllAlarmsResetingTimer = new DispatcherTimer();
			AllAlarmsResetingTimer.Interval = TimeSpan.FromSeconds(2);
			AllAlarmsResetingTimer.Tick += new EventHandler(AllAlarmsResetingTimer_Tick);
			AllAlarmsResetingTimer.Start();
			IsAllAlarmsReseting = true;
		}
		bool CanResetAll()
		{
			return !IsAllAlarmsReseting;
		}
		bool IsAllAlarmsReseting = false;
		DispatcherTimer AllAlarmsResetingTimer;
		void AllAlarmsResetingTimer_Tick(object sender, EventArgs e)
		{
			IsAllAlarmsReseting = false;
			AllAlarmsResetingTimer.Stop();
		}

		public RelayCommand RemoveAllFromIgnoreListCommand { get; private set; }
        void OnRemoveAllFromIgnoreList()
        {
            var devices = new List<Device>();
            foreach (var alarmViewModel in Alarms)
            {
                if (alarmViewModel.Alarm.AlarmType == AlarmType.Off)
                {
                    if (alarmViewModel.Alarm.Device != null)
                    {
                        if (FiresecManager.CanDisable(alarmViewModel.Alarm.Device.DeviceState) && alarmViewModel.Alarm.Device.DeviceState.IsDisabled)
                            devices.Add(alarmViewModel.Alarm.Device);
                    }
                }
            }

            if (ServiceFactory.SecurityService.Validate())
            {
				FiresecManager.FiresecDriver.RemoveFromIgnoreList(devices);
            }
        }
		public bool CanRemoveAllFromIgnoreList()
		{
			return Alarms.Any(x => x.Alarm.AlarmType == AlarmType.Off);
		}
	}
}