using System;
using System.IO;
using System.Runtime.Serialization;
using XFiresecAPI;
using Common;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		readonly static string XDeviceConfigurationFileName = "XDeviceConfiguration.xml";
		public static string ConfigurationDirectory(string FileNameOrDirectory)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", FileNameOrDirectory);
		}

		public void SetXDeviceConfiguration(XDeviceConfiguration xDeviceConfiguration)
		{
			var dataContractSerializer = new DataContractSerializer(typeof(XDeviceConfiguration));
			using (var fileStream = new FileStream(ConfigurationDirectory(XDeviceConfigurationFileName), FileMode.Create))
			{
				dataContractSerializer.WriteObject(fileStream, xDeviceConfiguration);
			}
		}

		public XDeviceConfiguration GetXDeviceConfiguration()
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(XDeviceConfiguration));
				using (var fileStream = new FileStream(ConfigurationDirectory(XDeviceConfigurationFileName), FileMode.Open))
				{
					return (XDeviceConfiguration)dataContractSerializer.ReadObject(fileStream);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.GetXDeviceConfiguration");
				var xDeviceConfiguration = new XDeviceConfiguration();
				var device = new XDevice();
				device.DriverUID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");
				xDeviceConfiguration.Devices.Add(device);
				xDeviceConfiguration.RootDevice = device;

				SetXDeviceConfiguration(xDeviceConfiguration);
				return xDeviceConfiguration;
			}
		}
	}
}