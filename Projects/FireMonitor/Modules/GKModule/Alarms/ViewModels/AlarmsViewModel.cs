using Common;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace GKModule.ViewModels
{
	public class AlarmsViewModel : ViewPartViewModel
	{
		public static AlarmsViewModel Current { get; private set; }
		List<Alarm> alarms;
		GKAlarmType? sortingAlarmType;

		public AlarmsViewModel()
		{
			Current = this;
			alarms = new List<Alarm>();
			Alarms = new ObservableCollection<AlarmViewModel>();
			ResetIgnoreAllCommand = new RelayCommand(OnResetIgnoreAll, CanResetIgnoreAll);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Unsubscribe(OnGKObjectsStateChanged);
			ServiceFactory.Events.GetEvent<GKObjectsStateChangedEvent>().Subscribe(OnGKObjectsStateChanged);
			OnGKObjectsStateChanged(null);
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
				OnPropertyChanged(() => SelectedAlarm);
			}
		}

		void OnGKObjectsStateChanged(object obj)
		{
			alarms = new List<Alarm>();
			foreach (var device in GKManager.Devices)
			{
				if (!device.IsRealDevice)
					continue;

				foreach (var stateClass in device.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, device));
							break;

						case XStateClass.Failure:
							alarms.Add(new Alarm(GKAlarmType.Failure, device));
							break;

						case XStateClass.On:
						case XStateClass.TurningOn:
							if (device.Driver.IsControlDevice)
							{
								if (!alarms.Any(x => x.AlarmType == GKAlarmType.Turning && x.GkBaseEntity.UID == device.UID))
								{
									alarms.Add(new Alarm(GKAlarmType.Turning, device));
								}
							}
							break;
					}
				}
				if (device.State.StateClasses.Contains(XStateClass.AutoOff) && device.Driver.IsControlDevice)
				{
					alarms.Add(new Alarm(GKAlarmType.AutoOff, device));
				}
				if (device.State.StateClasses.Contains(XStateClass.Service))
				{
					alarms.Add(new Alarm(GKAlarmType.Service, device));
				}
			}

			foreach (var zone in GKManager.Zones)
			{
				foreach (var stateClass in zone.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.Fire2:
							alarms.Add(new Alarm(GKAlarmType.Fire2, zone));
							break;

						case XStateClass.Fire1:
							alarms.Add(new Alarm(GKAlarmType.Fire1, zone));
							break;

						case XStateClass.Attention:
							alarms.Add(new Alarm(GKAlarmType.Attention, zone));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, zone));
							break;
					}
				}
			}

			foreach (var gGuardZone in GKManager.GuardZones)
			{
				foreach (var stateClass in gGuardZone.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.Fire1:
							alarms.Add(new Alarm(GKAlarmType.GuardAlarm, gGuardZone));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, gGuardZone));
							break;

						case XStateClass.Attention:
							alarms.Add(new Alarm(GKAlarmType.Attention, gGuardZone));
							break;
					}
				}
			}

			foreach (var door in GKManager.Doors)
			{
				foreach (var stateClass in door.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.Fire2:
							alarms.Add(new Alarm(GKAlarmType.Fire2, door));
							break;

						case XStateClass.Fire1:
							alarms.Add(new Alarm(GKAlarmType.GuardAlarm, door));
							break;

						case XStateClass.Attention:
							alarms.Add(new Alarm(GKAlarmType.Attention, door));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, door));
							break;

						case XStateClass.AutoOff:
							alarms.Add(new Alarm(GKAlarmType.AutoOff, door));
							break;
					}
				}
			}

			foreach (var direction in GKManager.Directions)
			{
				foreach (var stateClass in direction.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.On:
						case XStateClass.TurningOn:
							alarms.Add(new Alarm(GKAlarmType.NPTOn, direction));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, direction));
							break;
					}
				}
				if (direction.State.StateClasses.Contains(XStateClass.AutoOff))
				{
					alarms.Add(new Alarm(GKAlarmType.AutoOff, direction));
				}
			}

			foreach (var pumpStation in GKManager.PumpStations)
			{
				foreach (var stateClass in pumpStation.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.On:
						case XStateClass.TurningOn:
							alarms.Add(new Alarm(GKAlarmType.NPTOn, pumpStation));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, pumpStation));
							break;
					}
				}
				if (pumpStation.State.StateClasses.Contains(XStateClass.AutoOff))
				{
					alarms.Add(new Alarm(GKAlarmType.AutoOff, pumpStation));
				}
			}

			foreach (var mpt in GKManager.MPTs)
			{
				foreach (var stateClass in mpt.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.On:
						case XStateClass.TurningOn:
							alarms.Add(new Alarm(GKAlarmType.NPTOn, mpt));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, mpt));
							break;
					}
				}
				if (mpt.State.StateClasses.Contains(XStateClass.AutoOff))
				{
					alarms.Add(new Alarm(GKAlarmType.AutoOff, mpt));
				}
			}

			foreach (var delay in GKManager.Delays)
			{
				foreach (var stateClass in delay.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.On:
						case XStateClass.TurningOn:
							alarms.Add(new Alarm(GKAlarmType.Turning, delay));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, delay));
							break;
					}
				}
				if (delay.State.StateClasses.Contains(XStateClass.AutoOff))
				{
					alarms.Add(new Alarm(GKAlarmType.AutoOff, delay));
				}
			}

			alarms = (from Alarm alarm in alarms orderby alarm.AlarmType select alarm).ToList();

			UpdateAlarms();
			AlarmGroupsViewModel.Current.Update(alarms);
		}

		public void Sort(GKAlarmType? alarmType)
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
			if (sortingAlarmType.HasValue)
			{
				Alarms.Clear();
				foreach (var alarm in alarms)
				{
					if (sortingAlarmType.Value == alarm.AlarmType)
						Alarms.Add(new AlarmViewModel(alarm));
				}
			}
			else
			{
				for (int i = 0; i < alarms.Count; i++)
				{
					var alarm = alarms[i];
					var alarmViewModel = Alarms.FirstOrDefault(x => x.Alarm.IsEqualTo(alarm));
					if (alarmViewModel == null)
					{
						var newAlarmViewModel = new AlarmViewModel(alarm);
						Alarms.Insert(i, newAlarmViewModel);
					}
					else
					{
						alarmViewModel.OnPropertyChanged(() => alarmViewModel.ObjectStateClass);
					}
				}
				for (int i = 0; i < Alarms.Count; i++)
				{
					var alarm = alarms.FirstOrDefault(x => x.IsEqualTo(Alarms[i].Alarm));
					if (alarm == null)
					{
						Alarms.RemoveAt(i);
						i--;
					}
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
				foreach (var device in GKManager.Devices)
				{
					if (!device.Driver.IsDeviceOnShleif)
						continue;

					if (device.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(device);
					}
				}

				foreach (var zone in GKManager.Zones)
				{
					if (zone.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(zone);
					}
				}

				foreach (var guardZones in GKManager.GuardZones)
				{
					if (guardZones.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_GuardZone_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(guardZones);
					}
				}

				foreach (var door in GKManager.Doors)
				{
					if (door.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Door_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(door);
					}
				}

				foreach (var direction in GKManager.Directions)
				{
					if (direction.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Directions_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(direction);
					}
				}

				foreach (var pumpStation in GKManager.PumpStations)
				{
					if (pumpStation.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_NS_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(pumpStation);
					}
				}

				foreach (var mpt in GKManager.MPTs)
				{
					if (mpt.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_MPT_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(mpt);
					}
				}

				foreach (var delay in GKManager.Delays)
				{
					if (delay.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Delay_Control))
					{
						ClientManager.FiresecService.GKSetAutomaticRegime(delay);
					}
				}
			}
		}
		bool CanResetIgnoreAll()
		{
			try
			{
				foreach (var device in GKManager.Devices)
				{
					if (!device.Driver.IsDeviceOnShleif)
						continue;

					if (device.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var zone in GKManager.Zones)
				{
					if (zone.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var guardZone in GKManager.GuardZones)
				{
					if (guardZone.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var door in GKManager.Doors)
				{
					if (door.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var direction in GKManager.Directions)
				{
					if (direction.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var pumpStation in GKManager.PumpStations)
				{
					if (pumpStation.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var mpt in GKManager.MPTs)
				{
					if (mpt.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var delay in GKManager.Delays)
				{
					if (delay.State.StateClasses.Contains(XStateClass.Ignore))
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
					foreach (var zone in GKManager.Zones)
					{
						if (zone.State.StateClasses.Contains(XStateClass.Fire1))
						{
							ClientManager.FiresecService.GKResetFire1(zone);
						}
						if (zone.State.StateClasses.Contains(XStateClass.Fire2))
						{
							ClientManager.FiresecService.GKResetFire2(zone);
						}
					}
					foreach (var guardZone in GKManager.GuardZones)
					{
						if (guardZone.State.StateClasses.Contains(XStateClass.Fire1))
						{
							ClientManager.FiresecService.GKReset(guardZone);
						}
					}
					foreach (var door in GKManager.Doors)
					{
						if (door.State.StateClasses.Contains(XStateClass.Fire1))
						{
							ClientManager.FiresecService.GKReset(door);
						}
					}
				}
			}
		}

		bool CanResetAll()
		{
			return GetAlarmsToResetCount() > 0;
		}

		public int GetAlarmsToResetCount()
		{
			int result = 0;
			foreach (var zone in GKManager.Zones)
			{
				if (zone.State.StateClasses.Contains(XStateClass.Fire1))
					result++;
				if (zone.State.StateClasses.Contains(XStateClass.Fire2))
					result++;
			}
			foreach (var zone in GKManager.GuardZones)
			{
				if (zone.State.StateClasses.Contains(XStateClass.Fire1))
					result++;
			}
			foreach (var device in GKManager.Devices)
			{
				if (device.DriverType == GKDriverType.RSR2_MAP4)
				{
					if (device.State.StateClasses.Contains(XStateClass.Fire1) || device.State.StateClasses.Contains(XStateClass.Fire2))
						result++;
				}
			}
			foreach (var door in GKManager.Doors)
			{
				if (door.State.StateClasses.Contains(XStateClass.Fire1))
					result++;
			}
			return result;
		}

		void IgnoreAllZonesAndDevicesInFire()
		{
			foreach (var zone in GKManager.Zones)
			{
				if ((zone.State.StateClass == XStateClass.Fire1 || zone.State.StateClass == XStateClass.Fire2) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
				{
					ClientManager.FiresecService.GKSetIgnoreRegime(zone);
				}
			}

			foreach (var device in GKManager.Devices)
			{
				if (device.IsRealDevice)
				{
					if ((device.State.StateClass == XStateClass.Fire1 || device.State.StateClass == XStateClass.Fire2) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
					{
						ClientManager.FiresecService.GKSetIgnoreRegime(device);
					}
				}
			}
		}

		void ShortcutService_KeyPressed(object sender, KeyEventArgs e)
		{
			try
			{
				if (e.Key == System.Windows.Input.Key.F1 && GlobalSettingsHelper.GlobalSettings.Monitor_F1_Enabled)
				{
					ManualPdfHelper.Show();
				}
				if (e.Key == System.Windows.Input.Key.F2 && GlobalSettingsHelper.GlobalSettings.Monitor_F2_Enabled)
				{
					if (ServiceFactory.SecurityService.Validate())
					{
						if (CanResetIgnoreAll())
							OnResetIgnoreAll();
					}
				}
				if (e.Key == System.Windows.Input.Key.F3 && GlobalSettingsHelper.GlobalSettings.Monitor_F3_Enabled)
				{
					if (ServiceFactory.SecurityService.Validate())
					{
						IgnoreAllZonesAndDevicesInFire();
					}
				}
				if (e.Key == System.Windows.Input.Key.F4 && GlobalSettingsHelper.GlobalSettings.Monitor_F4_Enabled)
				{
					ResetAll();
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex, "AlarmsViewModel.ShortcutService_KeyPressed");
			}
		}
	}
}