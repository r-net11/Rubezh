using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using XFiresecAPI;
using Common;

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

		public static DeviceLibraryConfiguration DeviceLibraryConfiguration { get; set; }
		public static SystemConfiguration SystemConfiguration { get; set; }
		public static SecurityConfiguration SecurityConfiguration { get; set; }

        public static void UpdateFiles()
        {
            try
            {
                FileHelper.Synchronize();
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.UpdateFiles");
                LoadingErrors.AppendLine(e.Message);
            }
        }

        public static void GetConfiguration()
        {
            try
            {
                SystemConfiguration = FiresecService.GetSystemConfiguration();
                if (SystemConfiguration == null)
                {
                    SystemConfiguration = new SystemConfiguration();
                    Logger.Error("FiresecManager.SystemConfiguration = null");
                    LoadingErrors.AppendLine("Нулевая системная конфигурация");
                }

                DeviceLibraryConfiguration = FiresecService.GetDeviceLibraryConfiguration();
                if (DeviceLibraryConfiguration == null)
                {
                    DeviceLibraryConfiguration = new DeviceLibraryConfiguration();
                    Logger.Error("FiresecManager.DeviceLibraryConfiguration = null");
                    LoadingErrors.AppendLine("Нулевая конфигурация библиотеки устройств");
                }

                XManager.XDeviceLibraryConfiguration = FiresecService.GetXDeviceLibraryConfiguration();
                if (XManager.XDeviceLibraryConfiguration == null)
                {
                    XManager.XDeviceLibraryConfiguration = new XDeviceLibraryConfiguration();
                    Logger.Error("FiresecManager.XDeviceLibraryConfiguration = null");
                    LoadingErrors.AppendLine("Нулевая конфигурация библиотеки устройств ГК");
                }

                PlansConfiguration = FiresecService.GetPlansConfiguration();
                if (PlansConfiguration == null)
                {
                    PlansConfiguration = new PlansConfiguration();
                    Logger.Error("FiresecManager.PlansConfiguration = null");
                    LoadingErrors.AppendLine("Нулевая конфигурация графических планов");
                }

                SecurityConfiguration = FiresecService.GetSecurityConfiguration();
                if (SecurityConfiguration == null)
                {
                    SecurityConfiguration = new SecurityConfiguration();
                    Logger.Error("FiresecManager.SecurityConfiguration = null");
                    LoadingErrors.AppendLine("Нулевая конфигурация безопасности");
                }

                var driversConfiguration = FiresecService.GetDriversConfiguration();
                if ((driversConfiguration == null) || (driversConfiguration.Drivers == null) || (driversConfiguration.Drivers.Count == 0))
                {
                    driversConfiguration = new DriversConfiguration();
                    Logger.Error("FiresecManager.driversConfiguration = null");
                    MessageBox.Show("Нулевая конфигурация драйверов");
                }

                var deviceConfiguration = FiresecService.GetDeviceConfiguration();
                if (deviceConfiguration == null)
                {
                    deviceConfiguration = new DeviceConfiguration();
                    Logger.Error("FiresecManager.deviceConfiguration = null");
                    LoadingErrors.AppendLine("Нулевая конфигурация устройств");
                }

                FiresecConfiguration = new FiresecConfiguration()
                {
                    DriversConfiguration = driversConfiguration,
                    DeviceConfiguration = deviceConfiguration
                };

                UpdateConfiguration();
                FiresecConfiguration.CreateStates();
            }
            catch (Exception e)
            {
				Logger.Error(e, "FiresecManager.GetConfiguration");
                LoadingErrors.AppendLine(e.Message);
            }
        }

        public static void UpdateConfiguration()
        {
            try
            {
                PlansConfiguration.Update();
                FiresecConfiguration.UpdateConfiguration();
                UpdatePlansConfiguration();
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.UpdateConfiguration");
                LoadingErrors.AppendLine(e.Message);
            }
        }

        public static void UpdatePlansConfiguration()
        {
            try
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
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.UpdatePlansConfiguration");
                LoadingErrors.AppendLine(e.Message);
            }
        }

		public static void SetEmptyConfiguration()
		{
			FiresecConfiguration.SetEmptyConfiguration();
		}

		public static List<Driver> Drivers
		{
			get
            {
                if (FiresecConfiguration == null || FiresecConfiguration.DriversConfiguration == null || FiresecConfiguration.DriversConfiguration.Drivers == null)
                {
                    Logger.Error("FiresecManager Drivers = null");
                    return new List<Driver>();
                }
                return FiresecConfiguration.DriversConfiguration.Drivers;
            }
		}

        public static List<XDriver> XDrivers
        {
            get
            {
                if (FiresecConfiguration == null || FiresecConfiguration.XDriversConfiguration == null || FiresecConfiguration.XDriversConfiguration.XDrivers == null)
                {
                    Logger.Error("FiresecManager XDrivers = null");
                    return new List<XDriver>();
                }
                return FiresecConfiguration.XDriversConfiguration.XDrivers;
            }
        }

		public static List<Device> Devices
		{
			get
            {
                if (FiresecConfiguration == null || FiresecConfiguration.DeviceConfiguration == null || FiresecConfiguration.DeviceConfiguration.Devices == null)
                {
                    Logger.Error("FiresecManager Devices = null");
                    return new List<Device>();
                }
                return FiresecConfiguration.DeviceConfiguration.Devices;
            }
		}

		public static List<Zone> Zones
		{
			get
            {
                if (FiresecConfiguration == null || FiresecConfiguration.DeviceConfiguration == null || FiresecConfiguration.DeviceConfiguration.Zones == null)
                {
                    Logger.Error("FiresecManager Zones = null");
                    return new List<Zone>();
                }
                return FiresecConfiguration.DeviceConfiguration.Zones;
            }
		}

		public static List<Direction> Directions
		{
			get
            {
                if (FiresecConfiguration == null || FiresecConfiguration.DeviceConfiguration == null || FiresecConfiguration.DeviceConfiguration.Directions == null)
                {
                    Logger.Error("FiresecManager Direction = null");
                    return new List<Direction>();
                }
                return FiresecConfiguration.DeviceConfiguration.Directions;
            }
		}

		public static List<GuardUser> GuardUsers
		{
			get
            {
                if (FiresecConfiguration == null || FiresecConfiguration.DeviceConfiguration == null || FiresecConfiguration.DeviceConfiguration.GuardUsers == null)
                {
                    Logger.Error("FiresecManager GuardUser = null");
                    return new List<GuardUser>();
                }
                return FiresecConfiguration.DeviceConfiguration.GuardUsers;
            }
		}
	}
}