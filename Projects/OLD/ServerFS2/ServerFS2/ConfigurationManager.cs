using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Ionic.Zip;

namespace ServerFS2
{
	public static class ConfigurationManager
	{
		public static DeviceConfiguration DeviceConfiguration { get; set; }
		public static DriversConfiguration DriversConfiguration { get; set; }

		public static void Load()
		{
			MetadataHelper.Initialize();

			var serverConfigName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			var folderName = AppDataFolderHelper.GetFolder("Server2");
			var configFileName = Path.Combine(folderName, "Config.fscp");
			if (Directory.Exists(folderName))
				Directory.Delete(folderName, true);
			Directory.CreateDirectory(folderName);
			File.Copy(serverConfigName, configFileName);

			var zipFile = ZipFile.Read(configFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var fileInfo = new FileInfo(configFileName);
			var unzipFolderPath = Path.Combine(fileInfo.Directory.FullName, "Unzip");
			zipFile.ExtractAll(unzipFolderPath);

			var configurationFileName = Path.Combine(unzipFolderPath, "DeviceConfiguration.xml");
			DeviceConfiguration = ZipSerializeHelper.DeSerialize<DeviceConfiguration>(configurationFileName);

			configurationFileName = Path.Combine(unzipFolderPath, "DriversConfiguration.xml");
			DriversConfiguration = ZipSerializeHelper.DeSerialize<DriversConfiguration>(configurationFileName);
			DriversConfiguration.Drivers.ForEach(x => x.Properties.RemoveAll(z => z.IsAUParameter));
			DriverConfigurationParametersHelper.CreateKnownProperties(DriversConfiguration.Drivers);
			AddTempStates();
			Update();
		}

		static void AddTempStates()
		{
			var driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.AM1_O);
			if (driver != null)
			{
				var state = driver.States.FirstOrDefault(x => x.Code == "Alarm_AM1O");
				if (state == null)
				{
					var alarmState = driver.States.FirstOrDefault(x => x.Code == "Alarm");
					if (alarmState != null)
					{
						state = new DriverState()
						{
							Code = "Alarm_AM1O",
							Id = "999",
							AffectChildren = alarmState.AffectChildren,
							AffectParent = alarmState.AffectParent,
							CanResetOnPanel = alarmState.CanResetOnPanel,
							IsAutomatic = alarmState.IsAutomatic,
							IsManualReset = alarmState.IsManualReset,
							Name = alarmState.Name,
							StateType = alarmState.StateType
						};
						driver.States.Add(state);
					}
				}
			}
		}

		public static List<Device> Devices
		{
			get { return DeviceConfiguration.Devices; }
		}

		public static List<Zone> Zones
		{
			get { return DeviceConfiguration.Zones; }
		}

		public static List<Driver> Drivers
		{
			get { return DriversConfiguration.Drivers; }
		}

		public static void Update()
		{
			DeviceConfiguration.Update();
			DeviceConfiguration.Reorder();
			foreach (var device in Devices)
			{
				device.Driver = DriversConfiguration.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					continue;
				}
			}
			DeviceConfiguration.InvalidateConfiguration();
			DeviceConfiguration.UpdateCrossReferences();
			foreach (var device in Devices)
			{
				device.UpdateHasExternalDevices();
				device.DeviceState = new DeviceState()
				{
					DeviceUID = device.UID,
					Device = device
				};
			}

			foreach (var zone in DeviceConfiguration.Zones)
			{
				zone.ZoneState = new ZoneState()
				{
					Zone = zone,
					ZoneUID = zone.UID
				};
			}
		}
	}
}