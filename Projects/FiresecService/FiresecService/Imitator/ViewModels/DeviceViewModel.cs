﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecService.ViewModels
{
	public class DeviceViewModel : BaseViewModel
	{
		public DeviceViewModel(DeviceState deviceState)
		{
			DeviceState = deviceState;
			StateType = DeviceState.StateType;
			Name = DeviceState.Device.Driver.ShortName + " - " + DeviceState.Device.DottedAddress;
			Level = DeviceState.Device.PlaceInTree == null ? 0 : DeviceState.Device.PlaceInTree.Split('\\').Count() - 1;

			DriverStates = new List<DeviceStateViewModel>();
			foreach (var driverState in from x in DeviceState.Device.Driver.States orderby x.StateType select x)
			{
				if (!string.IsNullOrEmpty(driverState.Name))
				{
					var deviceStateViewModel = new DeviceStateViewModel(driverState);
					DriverStates.Add(deviceStateViewModel);
				}
			}

			foreach (var deviceDriverState in deviceState.States)
			{
				var state = DriverStates.FirstOrDefault(x => x.DriverState.Code == deviceDriverState.Code);
				state._isActive = true;
			}
		}

		public DeviceState DeviceState { get; private set; }
		public string Name { get; private set; }
		public StateType StateType { get; private set; }
		public List<DeviceStateViewModel> DriverStates { get; private set; }
		public int Level { get; private set; }
	}
}