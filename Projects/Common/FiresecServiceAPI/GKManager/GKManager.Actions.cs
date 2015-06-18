using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		public static void AddDevice(GKDevice device)
		{
			device.InitializeDefaultProperties();
		}

		#region RebuildRSR2Addresses
		public static void RebuildRSR2Addresses(GKDevice parentDevice)
		{
			foreach (var shliefDevice in parentDevice.Children)
			{
				RebuildRSR2Addresses_Children = new List<GKDevice>();
				RebuildRSR2Addresses_AddChild(shliefDevice);

				byte currentAddress = 1;
				foreach (var device in RebuildRSR2Addresses_Children)
				{
					device.IntAddress = currentAddress;
					if (!device.Driver.IsGroupDevice)
					{
						currentAddress++;
					}
					device.OnChanged();
				}
			}
		}

		static List<GKDevice> RebuildRSR2Addresses_Children;
		static void RebuildRSR2Addresses_AddChild(GKDevice device)
		{
			if (device.DriverType != GKDriverType.RSR2_MVP_Part && device.DriverType != GKDriverType.RSR2_KAU_Shleif)
				RebuildRSR2Addresses_Children.Add(device);

			foreach (var child in device.Children)
			{
				RebuildRSR2Addresses_AddChild(child);
			}
		}
		#endregion

		public static void ChangeLogic(GKDevice device, GKLogic logic)
		{
			device.Logic = logic;
			DeviceConfiguration.InvalidateOneLogic(device, device.Logic);
			device.OnChanged();
		}

		public static void ChangeDriver(GKDevice device, GKDriver driver)
		{
			var changeZone = !(driver.HasLogic);
			device.Driver = driver;
			device.DriverUID = driver.UID;
			if (driver.IsRangeEnabled)
				device.IntAddress = driver.MinAddress;

			device.Children.Clear();
			if (driver.IsGroupDevice)
			{
				var groupDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == device.Driver.GroupDeviceChildType);

				for (byte i = 0; i < device.Driver.GroupDeviceChildrenCount; i++)
				{
					var autoDevice = GKManager.AddChild(device, null, groupDriver, (byte)(device.IntAddress + i));
				}
			}

			if (changeZone)
			{
				ChangeLogic(device, new GKLogic());
			}
			device.Properties = new List<GKProperty>();
		}

		public static void RemoveSKDZone(GKSKDZone zone)
		{
			SKDZones.Remove(zone);
			zone.OnChanged();
		}

		public static void EditSKDZone(GKSKDZone zone)
		{
			zone.OnChanged();
		}
	}
}