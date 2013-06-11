using System.Collections.Generic;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Service;

namespace ServerFS2
{
	public static class StatesHelper
	{
		public static void ChangeDeviceStates(Device device, List<DeviceDriverState> states)
		{
			device.DeviceState.States = states;
			device.DeviceState.SerializableStates = device.DeviceState.States;
			CallbackManager.Add(new FS2Callbac() { ChangedDeviceStates = new List<DeviceState>() { device.DeviceState } });
			device.DeviceState.OnStateChanged();
		}
	}
}