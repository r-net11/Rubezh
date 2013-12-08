using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using System.Windows.Input;
using Common;
using System;
using Infrastructure.Common.Services;
using System.Diagnostics;

namespace GKModule.ViewModels
{
	public class AlarmsViewModel : ViewPartViewModel
	{
		public static AlarmsViewModel Current { get; private set; }
		List<Alarm> alarms;
		XAlarmType? sortingAlarmType;

		public AlarmsViewModel()
		{
			Current = this;
			alarms = new List<Alarm>();
			Alarms = new ObservableCollection<AlarmViewModel>();
			ResetIgnoreAllCommand = new RelayCommand(OnResetIgnoreAll, CanResetIgnoreAll);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Unsubscribe(OnGKObjectsStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Subscribe(OnGKObjectsStateChanged);
		}

		public void SubscribeShortcuts()
		{
			ApplicationService.Layout.ShortcutService.KeyPressed -= new KeyEventHandler(ShortcutService_KeyPressed);
			ApplicationService.Layout.ShortcutService.KeyPressed += new KeyEventHandler(ShortcutService_KeyPressed);
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

		void OnGKObjectsStateChanged(object obj)
		{
			alarms = new List<Alarm>();
			foreach (var device in XManager.Devices)
			{
				if ((device.Driver.IsGroupDevice)
					&& device.DriverType != XDriverType.GK && device.DriverType != XDriverType.KAU && device.DriverType != XDriverType.RSR2_KAU)
					continue;

				foreach (var stateClass in device.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.Ignore:
							alarms.Add(new Alarm(XAlarmType.Ignore, device));
							break;

						case XStateClass.Failure:
							alarms.Add(new Alarm(XAlarmType.Failure, device));
							break;

						case XStateClass.On:
						case XStateClass.TurningOn:
							if (device.Driver.IsControlDevice)
							{
								if (!alarms.Any(x => x.AlarmType == XAlarmType.Turning && x.Device.UID == device.UID))
								{
									alarms.Add(new Alarm(XAlarmType.Turning, device));
								}
							}
							break;

						case XStateClass.Fire1:
							alarms.Add(new Alarm(XAlarmType.Turning, device));
							break;

						case XStateClass.Fire2:
							if (device.DriverType != XDriverType.AM1_T)
							{
								alarms.Add(new Alarm(XAlarmType.Turning, device));
							}
							break;
					}
				}
				if (device.State.StateClasses.Contains(XStateClass.AutoOff) && device.Driver.IsControlDevice)
				{
					alarms.Add(new Alarm(XAlarmType.AutoOff, device));
				}
				if (device.State.StateClasses.Contains(XStateClass.Service)) // || device.DeviceState.IsRealMissmatch)
				{
					alarms.Add(new Alarm(XAlarmType.Service, device));
				}
			}

			foreach (var zone in XManager.Zones)
			{
				foreach (var stateClass in zone.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.Fire2:
							alarms.Add(new Alarm(XAlarmType.Fire2, zone));
							break;

						case XStateClass.Fire1:
							alarms.Add(new Alarm(XAlarmType.Fire1, zone));
							break;

						case XStateClass.Attention:
							alarms.Add(new Alarm(XAlarmType.Attention, zone));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(XAlarmType.Ignore, zone));
							break;
					}
				}

				//if (zone.ZoneState.IsRealMissmatch)
				//{
				//    alarms.Add(new Alarm(XAlarmType.Service, zone));
				//}
			}

			foreach (var direction in XManager.Directions)
			{
				foreach (var stateClass in direction.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.On:
						case XStateClass.TurningOn:
							alarms.Add(new Alarm(XAlarmType.NPTOn, direction));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(XAlarmType.Ignore, direction));
							break;
					}
				}
				if (direction.State.StateClasses.Contains(XStateClass.AutoOff))
				{
					alarms.Add(new Alarm(XAlarmType.AutoOff, direction));
				}

				//if (direction.DirectionState.IsRealMissmatch)
				//{
				//    alarms.Add(new Alarm(XAlarmType.Service, direction));
				//}
			}
			alarms = (from Alarm alarm in alarms orderby alarm.AlarmType select alarm).ToList();

			UpdateAlarms();
			AlarmGroupsViewModel.Current.Update(alarms);
		}

		public void Sort(XAlarmType? alarmType)
		{
			sortingAlarmType = alarmType;
			UpdateAlarms();
		}

		void UpdateAlarms()
		{
			Alarm oldAlarm = null;
			if (SelectedAlarm != null)
			{
				oldAlarm = SelectedAlarm.Alarm.Clone();
			}
			Alarms.Clear();
			foreach (var alarm in alarms)
			{
				if (!sortingAlarmType.HasValue || sortingAlarmType.Value == alarm.AlarmType)
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

		public RelayCommand ResetIgnoreAllCommand { get; private set; }
		void OnResetIgnoreAll()
		{
            if (ServiceFactory.SecurityService.Validate())
            {
				foreach (var device in XManager.Devices)
				{
					if (!device.Driver.IsDeviceOnShleif)
						continue;

					if (device.State.StateClasses.Contains(XStateClass.Ignore))
					{
						FiresecManager.FiresecService.GKSetAutomaticRegime(device);
					}
				}

				foreach (var zone in XManager.Zones)
				{
					if (zone.State.StateClasses.Contains(XStateClass.Ignore))
					{
						FiresecManager.FiresecService.GKSetAutomaticRegime(zone);
					}
				}

				foreach (var direction in XManager.Directions)
				{
					if (direction.State.StateClasses.Contains(XStateClass.Ignore))
					{
						FiresecManager.FiresecService.GKSetAutomaticRegime(direction);
					}
				}
			}
		}
		bool CanResetIgnoreAll()
		{
			try
			{
				if (XManager.Devices == null)
					Logger.Error("AlarmsViewModel XManager.Devices == null");
				if (XManager.Zones == null)
					Logger.Error("AlarmsViewModel XManager.Zones == null");
				if (XManager.Directions == null)
					Logger.Error("AlarmsViewModel XManager.Directions == null");

				foreach (var device in XManager.Devices)
				{
					if (!device.Driver.IsDeviceOnShleif)
						continue;

					if (device.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var zone in XManager.Zones)
				{
					if (zone.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var direction in XManager.Directions)
				{
					if (direction.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

        public void ResetAll()
        {
            if (CanResetAll())
            {
                if (ServiceFactory.SecurityService.Validate())
                {
                    foreach (var zone in XManager.Zones)
                    {
                        if (zone.State.StateClasses.Contains(XStateClass.Fire1))
                        {
							FiresecManager.FiresecService.GKResetFire1(zone);
                        }
                        if (zone.State.StateClasses.Contains(XStateClass.Fire2))
                        {
							FiresecManager.FiresecService.GKResetFire2(zone);
                        }
                    }
                    foreach (var device in XManager.Devices)
                    {
                        if (device.DriverType == XDriverType.AMP_1)
                        {
                            if (device.State.StateClasses.Contains(XStateClass.Fire1) || device.State.StateClasses.Contains(XStateClass.Fire2))
                            {
								FiresecManager.FiresecService.GKReset(device);
                            }
                        }
                    }
                }
            }
        }
		bool CanResetAll()
		{
			foreach (var zone in XManager.Zones)
			{
				if (zone.State.StateClasses.Contains(XStateClass.Fire1))
					return true;
				if (zone.State.StateClasses.Contains(XStateClass.Fire2))
					return true;
			}
            foreach (var device in XManager.Devices)
            {
                if (device.DriverType == XDriverType.AMP_1)
                {
                    if (device.State.StateClasses.Contains(XStateClass.Fire1) || device.State.StateClasses.Contains(XStateClass.Fire2))
                        return true;
                }
            }
			return false;
		}

		void ShortcutService_KeyPressed(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.Key == System.Windows.Input.Key.F1 && GlobalSettingsHelper.GlobalSettings.Monitor_F1_Enabled)
				{
				}
				if (e.Key == System.Windows.Input.Key.F2 && GlobalSettingsHelper.GlobalSettings.Monitor_F2_Enabled)
				{
				}
				if (e.Key == System.Windows.Input.Key.F3 && GlobalSettingsHelper.GlobalSettings.Monitor_F3_Enabled)
				{
				}
				if (e.Key == System.Windows.Input.Key.F4 && GlobalSettingsHelper.GlobalSettings.Monitor_F4_Enabled)
				{
					ResetAll();
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "XAlarmsViewModel.ShortcutService_KeyPressed");
			}
		}
	}
}