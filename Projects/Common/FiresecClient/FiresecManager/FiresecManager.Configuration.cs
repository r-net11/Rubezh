using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static FiresecConfiguration FiresecConfiguration { get; set; }
		public static PlansConfiguration PlansConfiguration
		{
            get { return FiresecAPI.Models.ConfigurationCash.PlansConfiguration; }
            set { FiresecAPI.Models.ConfigurationCash.PlansConfiguration = value; }
		}

		public static LibraryConfiguration LibraryConfiguration { get; set; }
		public static SystemConfiguration SystemConfiguration { get; set; }
		public static SecurityConfiguration SecurityConfiguration { get; set; }

        public static void GetConfiguration(bool updateFiles)
		{
			if (updateFiles)
				FileHelper.Synchronize();

			SystemConfiguration = FiresecService.GetSystemConfiguration();
			LibraryConfiguration = FiresecService.GetLibraryConfiguration();
			PlansConfiguration = FiresecService.GetPlansConfiguration();
			SecurityConfiguration = FiresecService.GetSecurityConfiguration();
			var driversConfiguration = FiresecService.GetDriversConfiguration();
			if ((driversConfiguration == null) || (driversConfiguration.Drivers == null) || (driversConfiguration.Drivers.Count == 0))
			{
				MessageBox.Show("Ошибка. Список драйверов пуст");
			}
			var deviceConfiguration = FiresecService.GetDeviceConfiguration();
			FiresecConfiguration = new FiresecConfiguration()
			{
				DriversConfiguration = driversConfiguration,
				DeviceConfiguration = deviceConfiguration
			};

			UpdateConfiguration();
			FiresecConfiguration.CreateStates();
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

		public static void SetEmptyConfiguration()
		{
			FiresecConfiguration.SetEmptyConfiguration();
		}

		public static List<Driver> Drivers
		{
			get { return FiresecConfiguration.DriversConfiguration.Drivers; }
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