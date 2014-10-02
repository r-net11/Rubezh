using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		public static GKDevice CopyDevice(GKDevice device, bool fullCopy)
		{
			var newDevice = new GKDevice();
			CopyDevice(device, newDevice);

			if (fullCopy)
			{
				newDevice.UID = device.UID;
			}

			return newDevice;
		}

		public static GKDevice CopyDevice(GKDevice deviceFrom, GKDevice deviceTo)
		{
			deviceTo.DriverUID = deviceFrom.DriverUID;
			deviceTo.Driver = deviceFrom.Driver;
			deviceTo.IntAddress = deviceFrom.IntAddress;
			deviceTo.Description = deviceFrom.Description;
			deviceTo.PredefinedName = deviceFrom.PredefinedName;

			deviceTo.Properties = new List<GKProperty>();
			foreach (var property in deviceFrom.Properties)
			{
				deviceTo.Properties.Add(new GKProperty()
				{
					Name = property.Name,
					Value = property.Value,
					DriverProperty = property.DriverProperty
				});
			}

			deviceTo.ZoneUIDs = deviceFrom.ZoneUIDs.ToList();
			deviceTo.Zones = deviceFrom.Zones.ToList();

			deviceTo.DeviceLogic.ClausesGroup = deviceFrom.DeviceLogic.ClausesGroup.Clone();
			deviceTo.DeviceLogic.OffClausesGroup = deviceFrom.DeviceLogic.OffClausesGroup.Clone();

			deviceTo.Children = new List<GKDevice>();
			foreach (var childDevice in deviceFrom.Children)
			{
				var newChildDevice = CopyDevice(childDevice, false);
				newChildDevice.Parent = deviceTo;
				deviceTo.Children.Add(newChildDevice);
			}

			deviceTo.PlanElementUIDs = new List<Guid>();
			foreach (var deviceElementUID in deviceFrom.PlanElementUIDs)
				deviceTo.PlanElementUIDs.Add(deviceElementUID);

			return deviceTo;
		}

		public static GKDevice AddChild(GKDevice parentDevice, GKDevice previousDevice, GKDriver driver, byte intAddress)
		{
			var device = new GKDevice()
			{
				DriverUID = driver.UID,
				Driver = driver,
				IntAddress = intAddress,
				Parent = parentDevice
			};
			device.InitializeDefaultProperties();

			if (previousDevice == null || parentDevice == previousDevice)
			{
				parentDevice.Children.Add(device);
			}
			else
			{
				var index = parentDevice.Children.IndexOf(previousDevice);
				parentDevice.Children.Insert(index + 1, device);
			}

			AddAutoCreateChildren(device);
			return device;
		}

		static void AddAutoCreateChildren(GKDevice device)
		{
			foreach (var autoCreateDriverType in device.Driver.AutoCreateChildren)
			{
				var autoCreateDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == autoCreateDriverType);
				for (byte i = autoCreateDriver.MinAddress; i <= autoCreateDriver.MaxAddress; i++)
				{
					AddChild(device, null, autoCreateDriver, i);
				}
			}
			if (device.DriverType == GKDriverType.GK)
			{
				DeviceConfiguration.UpdateGKPredefinedName(device);
			}
		}

		public static void SynchronizeChildern(GKDevice device)
		{
			for (int i = device.Children.Count(); i > 0; i--)
			{
				var childDevice = device.Children[i - 1];

				if (device.Driver.Children.Contains(childDevice.DriverType) == false)
				{
					device.Children.RemoveAt(i - 1);
				}
			}

			foreach (var autoCreateDriverType in device.Driver.AutoCreateChildren)
			{
				var autoCreateDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == autoCreateDriverType);
				for (byte i = autoCreateDriver.MinAddress; i <= autoCreateDriver.MaxAddress; i++)
				{
					var newDevice = new GKDevice()
					{
						DriverUID = autoCreateDriver.UID,
						Driver = autoCreateDriver,
						IntAddress = i
					};
					if (device.Children.Any(x => x.DriverType == newDevice.DriverType && x.Address == newDevice.Address) == false)
					{
						device.Children.Add(newDevice);
						newDevice.Parent = device;
					}
				}
			}
		}

		public static bool IsValidIpAddress(GKDevice device)
		{
			if (device.DriverType == GKDriverType.GK)
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