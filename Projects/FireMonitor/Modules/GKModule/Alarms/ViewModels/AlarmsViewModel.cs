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
			ResetAllCommand = new RelayCommand(OnResetAll, CanResetAll);
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
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if ((!device.Driver.IsDeviceOnShleif || device.Driver.IsGroupDevice)
					&& device.Driver.DriverType != XDriverType.GK && device.Driver.DriverType != XDriverType.KAU)
					continue;

				foreach (var stateType in device.DeviceState.States)
				{
					switch (stateType)
					{
						case XStateType.Ignore:
							alarms.Add(new Alarm(XAlarmType.Ignore, device));
							break;

						case XStateType.Failure:
							alarms.Add(new Alarm(XAlarmType.Failure, device));
							break;

						case XStateType.On:
						case XStateType.TurningOn:
							alarms.Add(new Alarm(XAlarmType.Turning, device));
							break;
					}
				}
				if (!device.DeviceState.States.Contains(XStateType.Norm) && !device.DeviceState.IsConnectionLost)
				{
					alarms.Add(new Alarm(XAlarmType.AutoOff, device));
				}
				if(device.DeviceState.IsService)
				{
					alarms.Add(new Alarm(XAlarmType.Service, device));
				}
			}

			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				foreach (var stateType in zone.ZoneState.States)
				{
					switch (stateType)
					{
						case XStateType.Fire2:
							alarms.Add(new Alarm(XAlarmType.Fire2, zone));
							break;

						case XStateType.Fire1:
							alarms.Add(new Alarm(XAlarmType.Fire1, zone));
							break;

						case XStateType.Attention:
							alarms.Add(new Alarm(XAlarmType.Attention, zone));
							break;

						case XStateType.Ignore:
							alarms.Add(new Alarm(XAlarmType.Ignore, zone));
							break;
					}
				}
			}

			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
				foreach (var stateType in direction.DirectionState.States)
				{
					switch (stateType)
					{
						case XStateType.On:
						case XStateType.TurningOn:
							alarms.Add(new Alarm(XAlarmType.NPTOn, direction));
							break;

						case XStateType.Ignore:
							alarms.Add(new Alarm(XAlarmType.Ignore, direction));
							break;
					}
				}
				if (!direction.DirectionState.States.Contains(XStateType.Norm) && !direction.DirectionState.IsConnectionLost)
				{
					alarms.Add(new Alarm(XAlarmType.AutoOff, direction));
				}
			}
			alarms = (from Alarm alarm in alarms orderby alarm.AlarmType select alarm).ToList();

			UpdateAlarms();
			AlarmGroupsViewModel.Current.Update(alarms);
			CheckInstructions();
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

		void CheckInstructions()
		{

		}

		public RelayCommand ResetIgnoreAllCommand { get; private set; }
		void OnResetIgnoreAll()
		{
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (!device.Driver.IsDeviceOnShleif)
					continue;

				if (device.DeviceState.States.Contains(XStateType.Ignore))
				{
					ObjectCommandSendHelper.SendControlCommand(device, 0x06);
				}
			}

			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (zone.ZoneState.States.Contains(XStateType.Ignore))
				{
					ObjectCommandSendHelper.SendControlCommand(zone, 0x06);
				}
			}

			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
				if (direction.DirectionState.States.Contains(XStateType.Ignore))
				{
					ObjectCommandSendHelper.SendControlCommand(direction, 0x06);
				}
			}
		}
		bool CanResetIgnoreAll()
		{
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (!device.Driver.IsDeviceOnShleif)
					continue;

				if (device.DeviceState.States.Contains(XStateType.Ignore))
					return true;
			}

			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (zone.ZoneState.States.Contains(XStateType.Ignore))
					return true;
			}

			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
				if (direction.DirectionState.States.Contains(XStateType.Ignore))
					return true;
			}
			return false;
		}

		public RelayCommand ResetAllCommand { get; private set; }
		void OnResetAll()
		{
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (zone.ZoneState.States.Contains(XStateType.Fire1))
				{
					ObjectCommandSendHelper.SendControlCommand(zone, 0x02);
				}
				if (zone.ZoneState.States.Contains(XStateType.Fire2))
				{
					ObjectCommandSendHelper.SendControlCommand(zone, 0x03);
				}
			}
		}
		bool CanResetAll()
		{
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (zone.ZoneState.States.Contains(XStateType.Fire1))
					return true;
				if (zone.ZoneState.States.Contains(XStateType.Fire2))
					return true;
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
					if (CanResetAll())
						OnResetAll();
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "XAlarmsViewModel.ShortcutService_KeyPressed");
			}
		}
	}
}