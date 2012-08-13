using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using System.Windows;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static FiresecConfiguration FiresecConfiguration { get; set; }

		public static DeviceConfigurationStates DeviceStates { get; set; }
		public static LibraryConfiguration LibraryConfiguration { get; set; }
		public static SystemConfiguration SystemConfiguration { get; set; }
		public static PlansConfiguration PlansConfiguration { get; set; }
		public static SecurityConfiguration SecurityConfiguration { get; set; }

		public static void GetConfiguration(bool updateFiles = true)
		{
			if (updateFiles)
				FileHelper.Synchronize();

			SystemConfiguration = FiresecService.GetSystemConfiguration();
			LibraryConfiguration = FiresecService.GetLibraryConfiguration();
			PlansConfiguration = FiresecService.GetPlansConfiguration();
			SecurityConfiguration = FiresecService.GetSecurityConfiguration();
			var drivers = FiresecService.GetDrivers();
			if (drivers == null)
			{
				MessageBox.Show("Ошибка. Список драйверов пуст");
			}
			var deviceConfiguration = FiresecService.GetDeviceConfiguration();
			FiresecConfiguration = new FiresecConfiguration()
			{
				Drivers = drivers,
				DeviceConfiguration = deviceConfiguration
			};

			FiresecConfiguration.UpdateDrivers();
			UpdateConfiguration();
		}

		public static void UpdateConfiguration()
		{
			PlansConfiguration.Update();
			FiresecConfiguration.UpdateConfiguration();
			UpdatePlansConfiguration();
		}

		public static void UpdatePlansConfiguration()
		{
			FiresecConfiguration.DeviceConfiguration.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				for (int i = plan.ElementDevices.Count(); i > 0; i--)
				{
					var elementDevice = plan.ElementDevices[i - 1];
					var device = FiresecConfiguration.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
					if (device != null)
						device.PlanElementUIDs.Add(elementDevice.UID);
				}
				for (int i = plan.ElementXDevices.Count(); i > 0; i--)
				{
					var elementXDevice = plan.ElementXDevices[i - 1];
					var xdevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementXDevice.XDeviceUID);
					if (xdevice != null)
						xdevice.PlanElementUIDs.Add(elementXDevice.UID);
				}
			}
		}

		public static List<Driver> Drivers
		{
			get { return FiresecConfiguration.Drivers; }
		}

		public static List<Device> Devices
		{
			get { return FiresecConfiguration.DeviceConfiguration.Devices; }
		}

		public static List<Zone> Zones
		{
			get { return FiresecConfiguration.DeviceConfiguration.Zones; }
		}

		public static List<Direction> Directions
		{
			get { return FiresecConfiguration.DeviceConfiguration.Directions; }
		}

		public static List<GuardUser> GuardUsers
		{
			get { return FiresecConfiguration.DeviceConfiguration.GuardUsers; }
		}
	}
}