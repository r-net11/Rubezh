using System;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public static class NewDeviceHelper
	{
		public static byte GetMinAddress(XDriver driver, XDevice device, byte shleifNo)
		{
			XDevice parentDevice = device;
			if (parentDevice.Driver.DriverType == XDriverType.MPT)
				parentDevice = parentDevice.Parent;

			byte maxAddress = 0;

			if (driver.IsRangeEnabled)
			{
				maxAddress = driver.MinAddress;
			}
			else
			{
				if (parentDevice.Driver.IsGroupDevice)
				{
					maxAddress = (byte)parentDevice.IntAddress;
				}
			}

			foreach (var child in parentDevice.Children)
			{
				if (child.Driver.IsAutoCreate)
					continue;

				if (driver.IsRangeEnabled)
				{
					if ((child.IntAddress < driver.MinAddress) && (child.IntAddress > driver.MaxAddress))
						continue;
				}

				if (child.ShleifNo != shleifNo)
					continue;

				if (child.Driver.IsGroupDevice)
				{
					if (child.IntAddress + child.Driver.GroupDeviceChildrenCount - 1 > maxAddress)
						maxAddress = (byte)Math.Min(255, child.IntAddress + child.Driver.GroupDeviceChildrenCount - 1);
				}

				if (child.IntAddress > maxAddress)
					maxAddress = (byte)child.IntAddress;

				if (child.Driver.DriverType == XDriverType.MPT)
				{
					foreach (var childMPT in child.Children)
					{
						if (childMPT.IntAddress > maxAddress)
							maxAddress = (byte)childMPT.IntAddress;
					}
				}
			}

			if (driver.IsRangeEnabled)
			{
				if (parentDevice.Children.Where(x => x.Driver.IsAutoCreate == false).Count() > 0)
					if (maxAddress + 1 <= driver.MaxAddress)
						maxAddress = (byte)(maxAddress + 1);
			}
			else
			{
				if (parentDevice.Driver.IsGroupDevice)
				{
					if (parentDevice.Children.Count > 0)
						if (maxAddress + 1 <= parentDevice.IntAddress + driver.GroupDeviceChildrenCount - 1)
							maxAddress = (byte)(maxAddress + 1);
				}
				else
				{
					if (parentDevice.Children.Where(x => x.Driver.IsAutoCreate == false).ToList().Count > 0)
						if (((maxAddress + 1) % 256) != 0)
							maxAddress = (byte)(maxAddress + 1);
				}
			}

			return Math.Max((byte)1, maxAddress);
		}

		public static DeviceViewModel AddDevice(XDevice xDevice, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(xDevice);
			parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in xDevice.Children)
			{
				AddDevice(childDevice, deviceViewModel);
			}

			if (xDevice.Driver.IsGroupDevice)
			{
				var driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == xDevice.Driver.GroupDeviceChildType);

				for (byte i = 0; i < xDevice.Driver.GroupDeviceChildrenCount; i++)
				{
					var autoDevice = XManager.AddChild(xDevice, driver, xDevice.ShleifNo, (byte)(xDevice.IntAddress + i));
					AddDevice(autoDevice, deviceViewModel);
				}
			}
			return deviceViewModel;
		}

		public static DeviceViewModel InsertDevice(XDevice device, DeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new DeviceViewModel(device);
			parentDeviceViewModel.InsertChild(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				InsertDevice(childDevice, deviceViewModel);
			}

			if (device.Driver.IsGroupDevice)
			{
				var driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == device.Driver.GroupDeviceChildType);

				for (byte i = 0; i < device.Driver.GroupDeviceChildrenCount; i++)
				{
					var autoDevice = XManager.AddChild(device, driver, device.ShleifNo, (byte)(device.IntAddress + i));
					AddDevice(autoDevice, deviceViewModel);
				}
			}
			return deviceViewModel;
		}
	}
}