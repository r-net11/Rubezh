using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public static class NewDeviceHelper
    {
        public static int GetMinAddress(Driver driver, Device parentDevice)
        {
            if (driver.UseParentAddressSystem)
            {
                if (driver.DriverType == DriverType.MPT)
                {
					while (parentDevice.Driver.UseParentAddressSystem)
                    {
						parentDevice = parentDevice.Parent;
                    }
                }
                else
                {
					parentDevice = parentDevice.Parent;
                }
            }

            int maxAddress = 0;

            if (driver.IsRangeEnabled)
            {
                maxAddress = driver.MinAddress;
            }
            else
            {
                if (parentDevice.Driver.ShleifCount > 0)
                    maxAddress = 257;

                if (parentDevice.Driver.IsChildAddressReservedRange)
                {
                    maxAddress = parentDevice.IntAddress;
                }
            }

			foreach (var child in FiresecManager.FiresecConfiguration.GetAllChildrenForDevice(parentDevice))
            {
                if (child.Driver.IsAutoCreate)
                    continue;

                if (driver.IsRangeEnabled)
                {
                    if ((child.IntAddress < driver.MinAddress) || (child.IntAddress > driver.MaxAddress))
                        continue;
                }

                if (parentDevice.Driver.IsChildAddressReservedRange)
                {
                    int reservedCount = parentDevice.GetReservedCount();

                    if ((child.IntAddress < parentDevice.IntAddress) && (child.IntAddress > parentDevice.IntAddress + reservedCount - 1))
                        continue;
                }

                if (child.Driver.AutoChildCount > 0)
                {
                    if (child.IntAddress + child.Driver.AutoChildCount - 1 > maxAddress)
                        maxAddress = child.IntAddress + child.Driver.AutoChildCount - 1;
                }
                else
                {
                    if (child.IntAddress > maxAddress)
                        maxAddress = child.IntAddress;
                }

				if (child.Driver.DriverType == DriverType.MRK_30)
				{
					maxAddress = child.IntAddress + child.GetReservedCount();
				}
            }

            if (parentDevice.Driver.DriverType == DriverType.MRK_30)
                maxAddress = parentDevice.IntAddress;

            if (driver.IsRangeEnabled)
            {
                if (parentDevice.Children.Count > 0)
                    if (maxAddress + 1 <= driver.MaxAddress)
                        maxAddress = maxAddress + 1;
            }
            else
            {
                if (parentDevice.Driver.IsChildAddressReservedRange)
                {
                    int reservedCount = driver.ChildAddressReserveRangeCount;
                    if (parentDevice.Driver.DriverType == DriverType.MRK_30)
                    {
                        reservedCount = 30;

                        var reservedCountProperty = parentDevice.Properties.FirstOrDefault(x => x.Name == "MRK30ChildCount");
                        if (reservedCountProperty != null)
                        {
                            reservedCount = int.Parse(reservedCountProperty.Value);
                        }
                    }

                    if (parentDevice.Children.Count > 0)
                        if (maxAddress + 1 <= parentDevice.IntAddress + reservedCount - 1)
                            maxAddress = maxAddress + 1;
                }
                else
                {
                    if (parentDevice.Children.Where(x=>x.Driver.IsAutoCreate == false).ToList().Count > 0)
                        if (((maxAddress + 1) % 256) != 0)
                            maxAddress = maxAddress + 1;
                }
            }

            return maxAddress;
        }

		public static void AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
		{
			if (parentDeviceViewModel.Driver.DriverType == DriverType.MPT)
			{
				device.ZoneNo = parentDeviceViewModel.Device.ZoneNo;
			}
			var deviceViewModel = new DeviceViewModel(device, parentDeviceViewModel.Source)
			{
				Parent = parentDeviceViewModel
			};
			parentDeviceViewModel.Children.Add(deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddDevice(childDevice, deviceViewModel);
			}

			if (device.Driver.AutoChild != Guid.Empty)
			{
				var driver = FiresecManager.FiresecConfiguration.Drivers.FirstOrDefault(x => x.UID == device.Driver.AutoChild);

				for (int i = 0; i < device.Driver.AutoChildCount; i++)
				{
					var autoDevice = FiresecManager.FiresecConfiguration.AddChild(device, driver, device.IntAddress + i);
					AddDevice(autoDevice, deviceViewModel);
				}
			}
		}
    }
}