using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Common;
using FiresecAPI.Models;
using Ionic.Zip;
using XFiresecAPI;
using Infrastructure.Common;

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
                LoadingErrorManager.Add(e);
            }
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Close();
        }

        public static void GetConfiguration(string fileFullName, Stream stream)
        {
            try
            {
                FiresecConfiguration = new FiresecConfiguration();
                var configurationsList = new ConfigurationsList();
                if (File.Exists(fileFullName))
                    File.Delete(fileFullName);
                CopyStream(stream, File.Create(fileFullName));
                var unzip = ZipFile.Read(fileFullName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
                var xmlstream = new MemoryStream();
                var entry = unzip["Info.xml"];
                if (entry != null)
                {
                    entry.Extract(xmlstream);
                    xmlstream.Position = 0;
                    configurationsList = SerializeHelper.DeSerialize<ConfigurationsList>(xmlstream);
                }

                if (configurationsList == null)
                    return;

                if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "SystemConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
                {
                    SystemConfiguration = FiresecService.GetSystemConfiguration();
                    if (SystemConfiguration == null)
                    {
                        SystemConfiguration = new SystemConfiguration();
                        Logger.Error("FiresecManager.SystemConfiguration = null");
                        LoadingErrorManager.Add("Нулевая системная конфигурация");
                    }
                }

                if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "DeviceLibraryConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
                {
                    DeviceLibraryConfiguration = FiresecService.GetDeviceLibraryConfiguration();
                    if (DeviceLibraryConfiguration == null)
                    {
                        DeviceLibraryConfiguration = new DeviceLibraryConfiguration();
                        Logger.Error("FiresecManager.DeviceLibraryConfiguration = null");
                        LoadingErrorManager.Add("Нулевая конфигурация библиотеки устройств");
                    }
                }

                if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "XDeviceLibraryConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
                {
                    XManager.XDeviceLibraryConfiguration = FiresecService.GetXDeviceLibraryConfiguration();
                    if (XManager.XDeviceLibraryConfiguration == null)
                    {
                        XManager.XDeviceLibraryConfiguration = new XDeviceLibraryConfiguration();
                        Logger.Error("FiresecManager.XDeviceLibraryConfiguration = null");
                        LoadingErrorManager.Add("Нулевая конфигурация библиотеки устройств ГК");
                    }
                }

                if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "PlansConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
                {
                    PlansConfiguration = FiresecService.GetPlansConfiguration();
                    if (PlansConfiguration == null)
                    {
                        PlansConfiguration = new PlansConfiguration();
                        Logger.Error("FiresecManager.PlansConfiguration = null");
                        LoadingErrorManager.Add("Нулевая конфигурация графических планов");
                    }
                }

                if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "SecurityConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
                {
                    SecurityConfiguration = FiresecService.GetSecurityConfiguration();
                    if (SecurityConfiguration == null)
                    {
                        SecurityConfiguration = new SecurityConfiguration();
                        Logger.Error("FiresecManager.SecurityConfiguration = null");
                        LoadingErrorManager.Add("Нулевая конфигурация безопасности");
                    }
                }

                if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "DriversConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
                {
                    var driversConfiguration = FiresecService.GetDriversConfiguration();
                    if ((driversConfiguration == null) || (driversConfiguration.Drivers == null) ||
                        (driversConfiguration.Drivers.Count == 0))
                    {
                        driversConfiguration = new DriversConfiguration();
                        Logger.Error("FiresecManager.driversConfiguration = null");
                        MessageBox.Show("Нулевая конфигурация драйверов");
                    }
                    FiresecConfiguration.DriversConfiguration = driversConfiguration;
                }

                if (configurationsList.Configurations.FirstOrDefault(x => (x.Name == "DeviceConfiguration") && (x.MajorVersion == 1) && (x.MinorVersion == 1)) != null)
                {
                    var deviceConfiguration = FiresecService.GetDeviceConfiguration();
                    if (deviceConfiguration == null)
                    {
                        deviceConfiguration = new DeviceConfiguration();
                        Logger.Error("FiresecManager.deviceConfiguration = null");
                        LoadingErrorManager.Add("Нулевая конфигурация устройств");
                    }
                    FiresecConfiguration.DeviceConfiguration = deviceConfiguration;
                }

                UpdateConfiguration();
                FiresecConfiguration.CreateStates();
            }
            catch (Exception e)
            {
				Logger.Error(e, "FiresecManager.GetConfiguration");
                LoadingErrorManager.Add(e);
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
                LoadingErrorManager.Add(e);
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
                LoadingErrorManager.Add(e);
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