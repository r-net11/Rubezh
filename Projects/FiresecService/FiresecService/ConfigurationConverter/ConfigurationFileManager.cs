using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.Models;

namespace FiresecService.Configuration
{
	public static class ConfigurationFileManager
	{
		readonly static string SystemConfigurationFileName = "SystemConfiguration.xml";
		readonly static string DeviceLibraryConfigurationFileName = "DeviceLibrary.xml";
		readonly static string PlansConfigurationFileName = "PlansConfiguration.xml";
		readonly static string DeviceConfigurationFileName = "DeviceConfiguration.xml";
		readonly static string SecurityConfigurationFileName = "SecurityConfiguration.xml";

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

			//try
			//{
			//    if (File.Exists(DeviceConfigurationFileName))
			//    {
			//        using (var fileStream = new FileStream(ConfigurationDirectory(DeviceConfigurationFileName), FileMode.Open))
			//        {
			//            var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
			//            return (DeviceConfiguration)dataContractSerializer.ReadObject(fileStream);
			//        }
			//    }
			//}
			//catch (Exception e)
			//{
			//    Logger.Error(e, "Исключение при вызове ConfigurationFileManager.GetDeviceConfiguration");
			//}
			//var deviceConfiguration = new DeviceConfiguration();
			//var device = new Device();
			//device.DriverUID = new Guid(DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverType == DriverType.Computer).DriverId);
			//deviceConfiguration.Devices.Add(device);
			//deviceConfiguration.RootDevice = device;

			//SetDeviceConfiguration(deviceConfiguration);
			//return deviceConfiguration;
		}

		public static void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
		{
			Set<DeviceConfiguration>(deviceConfiguration, DeviceConfigurationFileName);
			//Directory.CreateDirectory("Configuration");

			//using (var memoryStream = new MemoryStream())
			//{
			//    var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
			//    dataContractSerializer.WriteObject(memoryStream, deviceConfiguration);

			//    using (var fileStream = new FileStream(ConfigurationDirectory(DeviceConfigurationFileName), FileMode.Create))
			//    {
			//        fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
			//    }
			//}
		}

		public static SystemConfiguration GetSystemConfiguration()
		{
			return Get<SystemConfiguration>(SystemConfigurationFileName);
			//try
			//{
			//    using (var fileStream = new FileStream(ConfigurationDirectory(SystemConfigurationFileName), FileMode.Open))
			//    {
			//        var dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
			//        return (SystemConfiguration)dataContractSerializer.ReadObject(fileStream);
			//    }
			//}
			//catch (Exception e)
			//{
			//    Logger.Error(e, "Исключение при вызове ConfigurationFileManager.GetSystemConfiguration");
			//    var systemConfiguration = new SystemConfiguration();
			//    SetSystemConfiguration(systemConfiguration);
			//    return systemConfiguration;
			//}
		}

		public static void SetSystemConfiguration(SystemConfiguration systemConfiguration)
		{
			Set<SystemConfiguration>(systemConfiguration, SystemConfigurationFileName);
			//using (var memoryStream = new MemoryStream())
			//{
			//    var dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
			//    dataContractSerializer.WriteObject(memoryStream, systemConfiguration);

			//    using (var fileStream = new FileStream(ConfigurationDirectory(SystemConfigurationFileName), FileMode.Create))
			//    {
			//        fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
			//    }
			//}
		}

		public static LibraryConfiguration GetLibraryConfiguration()
		{
			return Get<LibraryConfiguration>(DeviceLibraryConfigurationFileName);
			//try
			//{
			//    using (var fileStream = new FileStream(ConfigurationDirectory(DeviceLibraryConfigurationFileName), FileMode.Open))
			//    {
			//        var dataContractSerializer = new DataContractSerializer(typeof(LibraryConfiguration));
			//        return (LibraryConfiguration)dataContractSerializer.ReadObject(fileStream);
			//    }
			//}
			//catch (Exception e)
			//{
			//    Logger.Error(e, "Исключение при вызове ConfigurationFileManager.GetLibraryConfiguration");
			//    var libraryConfiguration = new LibraryConfiguration();
			//    SetLibraryConfiguration(libraryConfiguration);
			//    return libraryConfiguration;
			//}
		}

		public static void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
		{
			Set<LibraryConfiguration>(libraryConfiguration, DeviceLibraryConfigurationFileName);
			//using (var memoryStream = new MemoryStream())
			//{
			//    var dataContractSerializer = new DataContractSerializer(typeof(LibraryConfiguration));
			//    dataContractSerializer.WriteObject(memoryStream, libraryConfiguration);

			//    using (var fileStream = new FileStream(ConfigurationDirectory(DeviceLibraryConfigurationFileName), FileMode.Create))
			//    {
			//        fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
			//    }
			//}
		}

		public static PlansConfiguration GetPlansConfiguration()
		{
			return Get<PlansConfiguration>(PlansConfigurationFileName);
			//try
			//{
			//    using (var fileStream = new FileStream(ConfigurationDirectory(PlansConfigurationFileName), FileMode.Open))
			//    {
			//        var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
			//        return (PlansConfiguration)dataContractSerializer.ReadObject(fileStream);
			//    }
			//}
			//catch (Exception e)
			//{
			//    Logger.Error(e, "Исключение при вызове ConfigurationFileManager.GetPlansConfiguration");
			//    var plansConfiguration = new PlansConfiguration();
			//    SetPlansConfiguration(plansConfiguration);
			//    return plansConfiguration;
			//}
		}

		public static void SetPlansConfiguration(PlansConfiguration plansConfiguration)
		{
			Set<PlansConfiguration>(plansConfiguration, PlansConfigurationFileName);
			//using (var memoryStream = new MemoryStream())
			//{
			//    var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
			//    dataContractSerializer.WriteObject(memoryStream, plansConfiguration);

			//    using (var fileStream = new FileStream(ConfigurationDirectory(PlansConfigurationFileName), FileMode.Create))
			//    {
			//        fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
			//    }
			//}
		}

		public static SecurityConfiguration GetSecurityConfiguration()
		{
			return Get<SecurityConfiguration>(SecurityConfigurationFileName);
			//try
			//{
			//    using (var fileStream = new FileStream(ConfigurationDirectory(SecurityConfigurationFileName), FileMode.Open))
			//    {
			//        var dataContractSerializer = new DataContractSerializer(typeof(SecurityConfiguration));
			//        return (SecurityConfiguration)dataContractSerializer.ReadObject(fileStream);
			//    }
			//}
			//catch (Exception e)
			//{
			//    Logger.Error(e, "Исключение при вызове ConfigurationFileManager.GetSecurityConfiguration");
			//    var securityConfiguration = new SecurityConfiguration();
			//    SetSecurityConfiguration(securityConfiguration);
			//    return securityConfiguration;
			//}
		}

		public static void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
		{
			Set<SecurityConfiguration>(securityConfiguration, SecurityConfigurationFileName);
			//using (var memoryStream = new MemoryStream())
			//{
			//    var dataContractSerializer = new DataContractSerializer(typeof(SecurityConfiguration));
			//    dataContractSerializer.WriteObject(memoryStream, securityConfiguration);

			//    using (var fileStream = new FileStream(ConfigurationDirectory(SecurityConfigurationFileName), FileMode.Create))
			//    {
			//        fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
			//    }
			//}
		}

		public static T Get<T>(string fileName)
			where T : new()
		{
			try
			{
				if (File.Exists(ConfigurationDirectory(fileName)))
				{
					using (var fileStream = new FileStream(ConfigurationDirectory(fileName), FileMode.Open))
					{
						var dataContractSerializer = new DataContractSerializer(typeof(T));
						return (T)dataContractSerializer.ReadObject(fileStream);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ConfigurationFileManager.Get<T> typeof(T) = " + typeof(T).ToString());
			}
			T configuration = new T();
			Set<T>(configuration, fileName);
			return configuration;
		}

		public static void Set<T>(T configuration, string fileName)
		{
			try
			{
				if (!Directory.Exists("Configuration"))
					Directory.CreateDirectory("Configuration");

				using (var memoryStream = new MemoryStream())
				{
					var dataContractSerializer = new DataContractSerializer(typeof(T));
					dataContractSerializer.WriteObject(memoryStream, configuration);

					using (var fileStream = new FileStream(ConfigurationDirectory(fileName), FileMode.Create))
					{
						fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
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