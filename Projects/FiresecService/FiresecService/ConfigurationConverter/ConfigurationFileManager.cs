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
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));

				using (var fileStream = new FileStream(ConfigurationDirectory(DeviceConfigurationFileName), FileMode.Open))
				{
					return (DeviceConfiguration)dataContractSerializer.ReadObject(fileStream);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ConfigurationFileManager.GetDeviceConfiguration");
				var deviceConfiguration = new DeviceConfiguration();
				var device = new Device();
				device.DriverUID = new Guid(DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverType == DriverType.Computer).DriverId);
				deviceConfiguration.Devices.Add(device);
				deviceConfiguration.RootDevice = device;

				SetDeviceConfiguration(deviceConfiguration);
				return deviceConfiguration;
			}
		}

		public static void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
		{
			var dataContractSerializer = new DataContractSerializer(typeof(DeviceConfiguration));
			Directory.CreateDirectory("Configuration");

			using (var memoryStream = new MemoryStream())
			{
				dataContractSerializer.WriteObject(memoryStream, deviceConfiguration);

				using (var fileStream = new FileStream(ConfigurationDirectory(DeviceConfigurationFileName), FileMode.Create))
				{
					fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
				}
			}
		}

		public static SystemConfiguration GetSystemConfiguration()
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));

				using (var fileStream = new FileStream(ConfigurationDirectory(SystemConfigurationFileName), FileMode.Open))
				{
					return (SystemConfiguration)dataContractSerializer.ReadObject(fileStream);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ConfigurationFileManager.GetSystemConfiguration");
				var systemConfiguration = new SystemConfiguration();
				SetSystemConfiguration(systemConfiguration);
				return systemConfiguration;
			}
		}

		public static void SetSystemConfiguration(SystemConfiguration systemConfiguration)
		{
			var dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
			using (var memoryStream = new MemoryStream())
			{
				dataContractSerializer.WriteObject(memoryStream, systemConfiguration);

				using (var fileStream = new FileStream(ConfigurationDirectory(SystemConfigurationFileName), FileMode.Create))
				{
					fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
				}
			}
		}

		public static LibraryConfiguration GetLibraryConfiguration()
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(LibraryConfiguration));
				using (var fileStream = new FileStream(ConfigurationDirectory(DeviceLibraryConfigurationFileName), FileMode.Open))
				{
					return (LibraryConfiguration)dataContractSerializer.ReadObject(fileStream);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ConfigurationFileManager.GetLibraryConfiguration");
				var libraryConfiguration = new LibraryConfiguration();
				SetLibraryConfiguration(libraryConfiguration);
				return libraryConfiguration;
			}
		}

		public static void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration)
		{
			var dataContractSerializer = new DataContractSerializer(typeof(LibraryConfiguration));
			using (var memoryStream = new MemoryStream())
			{
				dataContractSerializer.WriteObject(memoryStream, libraryConfiguration);

				using (var fileStream = new FileStream(ConfigurationDirectory(DeviceLibraryConfigurationFileName), FileMode.Create))
				{
					fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
				}
			}
		}

		public static PlansConfiguration GetPlansConfiguration()
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
				using (var fileStream = new FileStream(ConfigurationDirectory(PlansConfigurationFileName), FileMode.Open))
				{
					return (PlansConfiguration)dataContractSerializer.ReadObject(fileStream);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ConfigurationFileManager.GetPlansConfiguration");
				var plansConfiguration = new PlansConfiguration();
				SetPlansConfiguration(plansConfiguration);
				return plansConfiguration;
			}
		}

		public static void SetPlansConfiguration(PlansConfiguration plansConfiguration)
		{
			var dataContractSerializer = new DataContractSerializer(typeof(PlansConfiguration));
			using (var memoryStream = new MemoryStream())
			{
				dataContractSerializer.WriteObject(memoryStream, plansConfiguration);

				using (var fileStream = new FileStream(ConfigurationDirectory(PlansConfigurationFileName), FileMode.Create))
				{
					fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
				}
			}
		}

		public static SecurityConfiguration GetSecurityConfiguration()
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(SecurityConfiguration));
				using (var fileStream = new FileStream(ConfigurationDirectory(SecurityConfigurationFileName), FileMode.Open))
				{
					return (SecurityConfiguration)dataContractSerializer.ReadObject(fileStream);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ConfigurationFileManager.GetSecurityConfiguration");
				var securityConfiguration = new SecurityConfiguration();
				SetSecurityConfiguration(securityConfiguration);
				return securityConfiguration;
			}
		}

		public static void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
		{
			var dataContractSerializer = new DataContractSerializer(typeof(SecurityConfiguration));
			using (var memoryStream = new MemoryStream())
			{
				dataContractSerializer.WriteObject(memoryStream, securityConfiguration);

				using (var fileStream = new FileStream(ConfigurationDirectory(SecurityConfigurationFileName), FileMode.Create))
				{
					fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
				}
			}
		}
	}
}