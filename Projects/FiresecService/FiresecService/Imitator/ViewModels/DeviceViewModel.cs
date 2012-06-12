using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.Models;

namespace FiresecService.ViewModels
{
	public class DeviceViewModel : INotifyPropertyChanged
	{
		FiresecService.Service.FiresecService FiresecService;

		public DeviceViewModel(DeviceState deviceState, FiresecService.Service.FiresecService firesecService)
		{
			FiresecService = firesecService;

			DeviceState = deviceState;

			DriverStates = new List<DeviceStateViewModel>(
				DeviceState.Device.Driver.States.Select(driverState => new DeviceStateViewModel(driverState, ChangeState))
			);

			foreach (var deviceDriverState in deviceState.States)
			{
				var state = DriverStates.FirstOrDefault(x => x.DriverState.Code == deviceDriverState.Code);
				state._isActive = true;
			}
		}

		public DeviceState DeviceState { get; private set; }

		public string Name
		{
			get { return DeviceState.Device.Driver.ShortName + " - " + DeviceState.Device.DottedAddress; }
		}

		public StateType StateType
		{
			get { return DeviceState.StateType; }
		}

		public List<DeviceStateViewModel> DriverStates { get; private set; }

		public void ChangeState()
		{
			var deviceStates = new List<DeviceState>();
			DeviceState.States = new List<DeviceDriverState>(
				DriverStates.Where(state => state.IsActive).
				Select(state => new DeviceDriverState()
				{
					Code = state.DriverState.Code,
					DriverState = state.DriverState.Copy(),
					Time = DateTime.Now
				})
			);
			deviceStates.Add(DeviceState);

			FiresecService.CallbackWrapper.DeviceStateChanged(deviceStates);
			CalculateZones();

			OnPropertyChanged("State");
		}

		void CalculateZones()
		{
			if (FiresecService.FiresecManager.DeviceConfigurationStates.ZoneStates == null)
				return;

			foreach (var zoneState in FiresecService.FiresecManager.DeviceConfigurationStates.ZoneStates)
			{
				StateType minZoneStateType = StateType.Norm;
				foreach (var deviceState in FiresecService.FiresecManager.DeviceConfigurationStates.DeviceStates.
					Where(x => x.Device.ZoneNo == zoneState.No && !x.Device.Driver.IgnoreInZoneState))
				{
					if (deviceState.StateType < minZoneStateType)
						minZoneStateType = deviceState.StateType;
				}

				if (FiresecService.FiresecManager.DeviceConfigurationStates.DeviceStates.
					Any(x => x.Device.ZoneNo == zoneState.No) == false)
					minZoneStateType = StateType.Unknown;

				if (zoneState.StateType != minZoneStateType)
				{
					zoneState.StateType = minZoneStateType;
					FiresecService.CallbackWrapper.ZoneStateChanged(zoneState);
				}
			}
		}

		public int Level
		{
			get { return DeviceState.Device.PlaceInTree.Split('\\').Count() - 1; }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}
