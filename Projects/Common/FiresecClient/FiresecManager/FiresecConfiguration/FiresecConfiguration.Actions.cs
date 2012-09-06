using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecConfiguration
	{
		public Device AddDevice(Device parentDevice, Driver driver, int intAddress)
		{
			var device = new Device()
			{
				DriverUID = driver.UID,
				Driver = driver,
				IntAddress = intAddress,
				Parent = parentDevice
			};
			if (parentDevice.Driver.DriverType == DriverType.MPT)
			{
				device.ZoneNo = parentDevice.ZoneNo;
			}
			parentDevice.Children.Add(device);
			AddAutoCreateChildren(device);
			AddAutoChildren(device);
			parentDevice.OnChanged();
			return device;
		}

		void AddAutoCreateChildren(Device device)
		{
			foreach (var autoCreateDriverId in device.Driver.AutoCreateChildren)
			{
				var autoCreateDriver = Drivers.FirstOrDefault(x => x.UID == autoCreateDriverId);

				for (int i = autoCreateDriver.MinAutoCreateAddress; i <= autoCreateDriver.MaxAutoCreateAddress; i++)
				{
					AddDevice(device, autoCreateDriver, i);
				}
			}
		}

		void AddAutoChildren(Device device)
		{
			if (device.Driver.AutoChild != Guid.Empty)
			{
				var driver = FiresecManager.FiresecConfiguration.Drivers.FirstOrDefault(x => x.UID == device.Driver.AutoChild);

				for (int i = 0; i < device.Driver.AutoChildCount; i++)
				{
					var autoDevice = AddDevice(device, driver, device.IntAddress + i);
				}
			}
		}

		public void RemoveDevice()
		{
		}
	}
}