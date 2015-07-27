using System;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;

namespace GKModule.ViewModels
{
	public static class NewDeviceHelper
	{
		public static DeviceViewModel AddDevice(GKDevice device, DeviceViewModel parentDeviceViewModel, bool addAutoCreate = true)
		{
			var deviceViewModel = new DeviceViewModel(device);
			parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddDevice(childDevice, deviceViewModel, addAutoCreate);
			}

			if (addAutoCreate)
			{
				if (device.Driver.IsGroupDevice)
				{
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == device.Driver.GroupDeviceChildType);

					for (byte i = 0; i < device.Driver.GroupDeviceChildrenCount; i++)
					{
						var autoDevice = GKManager.AddChild(device, null, driver, (byte)(device.IntAddress + i));
						AddDevice(autoDevice, deviceViewModel, addAutoCreate);
					}
				}
			}
			return deviceViewModel;
		}

		public static DeviceViewModel InsertDevice(GKDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			parentDeviceViewModel.InsertChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddDevice(childDevice, deviceViewModel);
			}

			if (device.Driver.IsGroupDevice && device.Children.Count == 0)
			{
				var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == device.Driver.GroupDeviceChildType);

				for (byte i = 0; i < device.Driver.GroupDeviceChildrenCount; i++)
				{
					var autoDevice = GKManager.AddChild(device, null, driver, (byte)(device.IntAddress + i));
					AddDevice(autoDevice, deviceViewModel);
				}
			}
			return deviceViewModel;
		}
	}
}