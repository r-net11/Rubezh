using System;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public static class NewDeviceHelper
	{
		public static DeviceViewModel AddDevice(GKDevice device, DeviceViewModel parentDeviceViewModel, bool isAddDevice = true)
		{
			var deviceViewModel = new DeviceViewModel(device);
			if (isAddDevice)
			{
				parentDeviceViewModel.AddChild(deviceViewModel);
				foreach (var childDevice in device.Children)
				{
					AddDevice(childDevice, deviceViewModel);
				}
			}
			else
			{
				parentDeviceViewModel.InsertChild(deviceViewModel);

				foreach (var childDevice in device.Children)
				{
					AddDevice(childDevice, deviceViewModel, !isAddDevice);
				}
			}

			//if (addAutoCreate)
			//{
			//	if (device.Driver.IsGroupDevice && device.DriverType != GKDriverType.RSR2_OPSZ)
			//	{
			//		var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == device.Driver.GroupDeviceChildType);

			//		for (byte i = 0; i < device.Driver.GroupDeviceChildrenCount; i++)
			//		{
			//			var autoDevice = GKManager.AddChild(device, null, driver, device.IntAddress + i);
			//			AddDevice(autoDevice, deviceViewModel, addAutoCreate);
			//		}
			//	}
			//}
			return deviceViewModel;
		}

		public static DeviceViewModel InsertDevice(GKDevice device, DeviceViewModel parentDeviceViewModel, bool addAutoCreate = true)
		{
			var deviceViewModel = new DeviceViewModel(device);
			parentDeviceViewModel.InsertChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddDevice(childDevice, deviceViewModel, addAutoCreate);
			}

			//if (device.Driver.IsGroupDevice && device.Children.Count == 0)
			//{
			//	var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == device.Driver.GroupDeviceChildType);

			//	for (byte i = 0; i < device.Driver.GroupDeviceChildrenCount; i++)
			//	{
			//		var autoDevice = GKManager.AddChild(device, null, driver, device.IntAddress + i);
			//		AddDevice(autoDevice, deviceViewModel);
			//	}
			//}
			return deviceViewModel;
		}
	}
}