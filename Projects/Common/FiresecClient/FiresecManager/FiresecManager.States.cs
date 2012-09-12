using System;
using System.Linq;
using System.Windows;
using Common;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		[Obsolete]
		public static DeviceConfigurationStates DeviceStates { get; set; }

		public static void GetStates()
		{
			//FiresecDriver.ConfigurationConverter.SynchronyzeConfiguration();
			//FiresecDriver.ConfigurationConverter.Update();
			DeviceStates = FiresecDriver.ConvertStates();

			UpdateStates();
			FiresecService.StartPing();
		}

		public static void UpdateStates()
		{
			foreach (var deviceState in DeviceStates.DeviceStates)
			{
				deviceState.Device = FiresecConfiguration.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceState.UID);
				if (deviceState.Device == null)
				{
					MessageBox.Show("Ошибка при сопоставлении устройства с его состоянием");
					continue;
				}

				foreach (var state in deviceState.States)
				{
					state.DriverState = deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == state.Code);
				}

				foreach (var parentState in deviceState.ParentStates)
				{
					parentState.ParentDevice = FiresecConfiguration.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == parentState.ParentDeviceId);
					if (parentState.ParentDevice != null)
						parentState.DriverState = parentState.ParentDevice.Driver.States.FirstOrDefault(x => x.Code == parentState.Code);
				}

				foreach (var childState in deviceState.ChildStates)
				{
					childState.ChildDevice = FiresecConfiguration.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == childState.ChildDeviceId);
					if (childState.ChildDevice != null)
						childState.DriverState = childState.ChildDevice.Driver.States.FirstOrDefault(x => x.Code == childState.Code);
				}
			}

			foreach (var zoneState in DeviceStates.ZoneStates)
			{
				zoneState.Zone = FiresecConfiguration.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneState.No);
				if (zoneState.Zone == null)
				{
					Logger.Error("FiresecManager.UpdateStates zoneState.Zone == null");
				}
			}
		}
	}
}