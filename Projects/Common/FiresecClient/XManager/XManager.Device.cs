using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class XManager
	{
		public static XDevice CopyDevice(XDevice device, bool fullCopy)
		{
			var newDevice = new XDevice();
			CopyDevice(device, newDevice);

			if (fullCopy)
			{
				newDevice.UID = device.UID;
			}

			return newDevice;
		}

		public static XDevice CopyDevice(XDevice deviceFrom, XDevice deviceTo)
		{
			deviceTo.DriverUID = deviceFrom.DriverUID;
			deviceTo.Driver = deviceFrom.Driver;
			deviceTo.ShleifNo = deviceFrom.ShleifNo;
			deviceTo.IntAddress = deviceFrom.IntAddress;
			deviceTo.Description = deviceFrom.Description;

			deviceTo.Properties = new List<XProperty>();
			foreach (var property in deviceFrom.Properties)
			{
				deviceTo.Properties.Add(new XProperty()
				{
					Name = property.Name,
					Value = property.Value
				});
			}

			deviceTo.Children = new List<XDevice>();
			foreach (var childDevice in deviceFrom.Children)
			{
				var newChildDevice = CopyDevice(childDevice, false);
				newChildDevice.Parent = deviceTo;
				deviceTo.Children.Add(newChildDevice);
			}

			return deviceTo;
		}

		public static XDevice AddChild(XDevice parentDevice, XDriver driver, byte shleifNo, byte intAddress)
		{
			var device = new XDevice()
			{
				DriverUID = driver.UID,
				Driver = driver,
				ShleifNo = shleifNo,
				IntAddress = intAddress,
				Parent = parentDevice
			};
			device.InitializeDefaultProperties();
			parentDevice.Children.Add(device);
			AddAutoCreateChildren(device);

			return device;
		}

		public static XDevice InsertChild(XDevice parentDevice, XDevice previousDevice, XDriver driver, byte shleifNo, byte intAddress)
		{
			var device = new XDevice()
			{
				DriverUID = driver.UID,
				Driver = driver,
				ShleifNo = shleifNo,
				IntAddress = intAddress,
				Parent = parentDevice
			};
			device.InitializeDefaultProperties();
			var index = parentDevice.Children.IndexOf(previousDevice);
			parentDevice.Children.Insert(index + 1, device);
			AddAutoCreateChildren(device);

			return device;
		}

		static void AddAutoCreateChildren(XDevice device)
		{
			foreach (var autoCreateDriverType in device.Driver.AutoCreateChildren)
			{
				var autoCreateDriver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == autoCreateDriverType);
				for (byte i = autoCreateDriver.MinAddress; i <= autoCreateDriver.MaxAddress; i++)
				{
					AddChild(device, autoCreateDriver, 0, i);
				}
			}
			if (device.Driver.DriverType == XDriverType.GK)
			{
				UpdateGKPredefinedName(device);
			}
		}

		public static void SynchronizeChildern(XDevice device)
		{
			for (int i = device.Children.Count(); i > 0; i--)
			{
				var childDevice = device.Children[i - 1];

				if (device.Driver.Children.Contains(childDevice.Driver.DriverType) == false)
				{
					device.Children.RemoveAt(i - 1);
				}
			}

			foreach (var autoCreateDriverType in device.Driver.AutoCreateChildren)
			{
				var autoCreateDriver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverType == autoCreateDriverType);
				for (byte i = autoCreateDriver.MinAddress; i <= autoCreateDriver.MaxAddress; i++)
				{
					var newDevice = new XDevice()
					{
						DriverUID = autoCreateDriver.UID,
						Driver = autoCreateDriver,
						IntAddress = i
					};
					if (device.Children.Any(x => x.Driver.DriverType == newDevice.Driver.DriverType && x.Address == newDevice.Address) == false)
					{
						device.Children.Add(newDevice);
						newDevice.Parent = device;
					}
				}
			}
		}

		public static bool IsValidIpAddress(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.GK)
			{
				const string pattern = @"^([01]\d\d?|[01]?[1-9]\d?|2[0-4]\d|25[0-3])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])$";
				var address = device.GetGKIpAddress();
				if (string.IsNullOrEmpty(address) || !Regex.IsMatch(address, pattern))
				{
					return false;
				}
			}
			return true;
		}
	}
}
