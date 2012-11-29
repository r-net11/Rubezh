using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using XFiresecAPI;
using Infrastructure.Common;

namespace FiresecService.Configuration
{
    public static class ConfigurationFileManager
    {
        readonly static string SystemConfigurationFileName = "SystemConfiguration.xml";
        readonly static string DeviceLibraryConfigurationFileName = "DeviceLibraryConfiguration.xml";
        readonly static string PlansConfigurationFileName = "PlansConfiguration.xml";
        readonly static string DeviceConfigurationFileName = "DeviceConfiguration.xml";
        readonly static string SecurityConfigurationFileName = "SecurityConfiguration.xml";
        readonly static string DriversConfigurationFileName = "DriversConfiguration.xml";
        readonly static string XDeviceConfigurationFileName = "XDeviceConfiguration.xml";
        readonly static string XDeviceLibraryConfigurationFileName = "XDeviceLibraryConfiguration.xml";

        public static string ConfigurationDirectory(string FileNameOrDirectory)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", FileNameOrDirectory);
        }

        public static DeviceConfiguration GetDeviceConfiguration()
        {
            var deviceConfiguration = Get<DeviceConfiguration>(DeviceConfigurationFileName);
            if (deviceConfiguration.RootDevice == null)
            {
                var device = new Device()
                {
                    DriverUID = new Guid(DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverType == DriverType.Computer).DriverId)
                };
                deviceConfiguration.Devices.Add(device);
                deviceConfiguration.RootDevice = device;

                SetDeviceConfiguration(deviceConfiguration);
            }
            return deviceConfiguration;
        }

        public static void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            Set<DeviceConfiguration>(deviceConfiguration, DeviceConfigurationFileName);
        }

        public static SystemConfiguration GetSystemConfiguration()
        {
            return Get<SystemConfiguration>(SystemConfigurationFileName);
        }

        public static void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            Set<SystemConfiguration>(systemConfiguration, SystemConfigurationFileName);
        }

        public static DeviceLibraryConfiguration GetDeviceLibraryConfiguration()
        {
            return Get<DeviceLibraryConfiguration>(DeviceLibraryConfigurationFileName);
        }

        public static void SetDeviceLibraryConfiguration(DeviceLibraryConfiguration deviceLibraryConfiguration)
        {
            Set<DeviceLibraryConfiguration>(deviceLibraryConfiguration, DeviceLibraryConfigurationFileName);
        }

        public static PlansConfiguration GetPlansConfiguration()
        {
            return Get<PlansConfiguration>(PlansConfigurationFileName);
        }

        public static void SetPlansConfiguration(PlansConfiguration plansConfiguration)
        {
            Set<PlansConfiguration>(plansConfiguration, PlansConfigurationFileName);
        }

        public static SecurityConfiguration GetSecurityConfiguration()
        {
            return Get<SecurityConfiguration>(SecurityConfigurationFileName);
        }

        public static void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            Set<SecurityConfiguration>(securityConfiguration, SecurityConfigurationFileName);
        }

        public static DriversConfiguration GetDriversConfiguration()
        {
            return Get<DriversConfiguration>(DriversConfigurationFileName);
        }

        public static void SetDriversConfiguration(DriversConfiguration driversConfiguration)
        {
            Set<DriversConfiguration>(driversConfiguration, DriversConfigurationFileName);
        }

        public static XDeviceLibraryConfiguration GetXDeviceLibraryConfiguration()
        {
            return Get<XDeviceLibraryConfiguration>(XDeviceLibraryConfigurationFileName);
        }

        public static void SetXDeviceLibraryConfiguration(XDeviceLibraryConfiguration xDeviceLibraryConfiguration)
        {
            Set<XDeviceLibraryConfiguration>(xDeviceLibraryConfiguration, XDeviceLibraryConfigurationFileName);
        }

        public static XDeviceConfiguration GetXDeviceConfiguration()
        {
            var deviceConfiguration = Get<XDeviceConfiguration>(XDeviceConfigurationFileName);
            if (deviceConfiguration.RootDevice == null)
            {
                var device = new XDevice();
                device.DriverUID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");
                deviceConfiguration.Devices.Add(device);
                deviceConfiguration.RootDevice = device;

                SetXDeviceConfiguration(deviceConfiguration);
            }
            return deviceConfiguration;
        }

        public static void SetXDeviceConfiguration(XDeviceConfiguration deviceConfiguration)
        {
            Set<XDeviceConfiguration>(deviceConfiguration, XDeviceConfigurationFileName);
        }

		public static T Get<T>(string fileName)
			where T : VersionedConfiguration, new()
		{
			try
			{
                var memStream = ConfigHelper.FromZip(fileName);
				if ((File.Exists(ConfigurationDirectory(fileName)))||( memStream != null))
				{
					T configuration = null;
                        if (memStream == null)
                        {
                            using (var fileStream = new FileStream(ConfigurationDirectory(fileName), FileMode.Open))
                            {
                                memStream = new MemoryStream();
                                memStream.SetLength(fileStream.Length);
                                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                            }
                        }

                    var dataContractSerializer = new DataContractSerializer(typeof (T));
                    configuration = (T) dataContractSerializer.ReadObject(memStream);

					if (!configuration.ValidateVersion())
					{
						Set<T>(configuration, fileName);
					}
					return configuration;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ConfigurationFileManager.Get<T> typeof(T) = " + typeof(T).ToString());
			}
			T newConfiguration = new T();
			Set<T>(newConfiguration, fileName);
			return newConfiguration;
		}

        public static void Set<T>(T configuration, string fileName)
            where T : VersionedConfiguration
        {
            try
            {
                configuration.Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 };

                if (!Directory.Exists("Configuration"))
                    Directory.CreateDirectory("Configuration");

                using (var memoryStream = new MemoryStream())
                {
                    var dataContractSerializer = new DataContractSerializer(typeof(T));
                    dataContractSerializer.WriteObject(memoryStream, configuration);

                    using (var fileStream = new FileStream(ConfigurationDirectory(fileName), FileMode.Create))
                    {
                        fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
                        ConfigHelper.IntoZip(fileName, memoryStream);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове ConfigurationFileManager.Set<T> typeof(T) = " + typeof(T).ToString());
            }
        }
    }
}