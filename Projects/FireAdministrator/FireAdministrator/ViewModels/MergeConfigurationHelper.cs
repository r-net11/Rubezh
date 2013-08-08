using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Infrastructure;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using System.IO;
using Infrastructure.Common;
using Common;
using FiresecAPI.Models;
using Ionic.Zip;
using System.Diagnostics;

namespace FireAdministrator.ViewModels
{
	public static class MergeConfigurationHelper
	{
		public static void Merge()
		{
			var openDialog = new OpenFileDialog()
			{
				Filter = "firesec2 files|*.fscp",
				DefaultExt = "firesec2 files|*.fscp"
			};
			if (openDialog.ShowDialog().Value)
			{
				ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
				ZipConfigActualizeHelper.Actualize(openDialog.FileName, false);
				var folderName = AppDataFolderHelper.GetLocalFolder("Administrator/MergeConfiguration");
				var configFileName = Path.Combine(folderName, "Config.fscp");
				if (Directory.Exists(folderName))
					Directory.Delete(folderName, true);
				Directory.CreateDirectory(folderName);
				File.Copy(openDialog.FileName, configFileName);
				LoadFromZipFile(configFileName);
				ServiceFactory.ContentService.Invalidate();

				FiresecManager.UpdateConfiguration();

				ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
				ServiceFactory.Layout.Close();
				if (ApplicationService.Modules.Any(x => x.Name == "Устройства, Зоны, Направления"))
					ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
				else if (ApplicationService.Modules.Any(x => x.Name == "Групповой контроллер"))
					ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Guid.Empty);

				ServiceFactory.SaveService.FSChanged = true;
				ServiceFactory.SaveService.PlansChanged = true;
				ServiceFactory.SaveService.GKChanged = true;
				ServiceFactory.Layout.ShowFooter(null);
			}
		}

		public static void LoadFromZipFile(string fileName)
		{
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var fileInfo = new FileInfo(fileName);
			var unzipFolderPath = Path.Combine(fileInfo.Directory.FullName, "Unzip");
			zipFile.ExtractAll(unzipFolderPath);

			var zipConfigurationItemsCollectionFileName = Path.Combine(unzipFolderPath, "ZipConfigurationItemsCollection.xml");
			if (!File.Exists(zipConfigurationItemsCollectionFileName))
			{
				Logger.Error("FiresecManager.LoadFromZipFile zipConfigurationItemsCollectionFileName file not found");
				return;
			}
			var zipConfigurationItemsCollection = ZipSerializeHelper.DeSerialize<ZipConfigurationItemsCollection>(zipConfigurationItemsCollectionFileName);
			if (zipConfigurationItemsCollection == null)
			{
				Logger.Error("FiresecManager.LoadFromZipFile zipConfigurationItemsCollection == null");
				return;
			}

			foreach (var zipConfigurationItem in zipConfigurationItemsCollection.GetWellKnownZipConfigurationItems)
			{
				var configurationFileName = Path.Combine(unzipFolderPath, zipConfigurationItem.Name);
				if (File.Exists(configurationFileName))
				{
					switch (zipConfigurationItem.Name)
					{
						case "DeviceConfiguration.xml":
							var deviceConfiguration = ZipSerializeHelper.DeSerialize<DeviceConfiguration>(configurationFileName);
							MergeDevices(deviceConfiguration);
							break;

						case "PlansConfiguration.xml":
							var plansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationFileName);
							MergePlans(plansConfiguration);
							break;
					}
				}
			}

			var destinationImagesDirectory = AppDataFolderHelper.GetLocalFolder("Administrator/Configuration/Unzip/Content");
			var sourceImagesDirectoryInfo = new DirectoryInfo(Path.Combine(unzipFolderPath, "Content"));
			foreach (var imageFileInfo in sourceImagesDirectoryInfo.GetFiles())
			{
				imageFileInfo.CopyTo(Path.Combine(destinationImagesDirectory, imageFileInfo.Name), true);
			}

			zipFile.Dispose();
		}

		static void MergeDevices(DeviceConfiguration deviceConfiguration)
		{
			deviceConfiguration.Update();
			foreach (var device in deviceConfiguration.Devices)
				device.Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
			foreach (var device in deviceConfiguration.Devices)
				if (device.Driver != null && (device.Driver.DriverType == DriverType.MS_1 || device.Driver.DriverType == DriverType.MS_2))
					AddDeviceTree(device);
			foreach (var device in deviceConfiguration.Devices)
				if (device.Driver != null && device.Driver.IsPanel)
					AddDeviceTree(device);
			var maxZoneNo = 0;
			if (FiresecManager.Zones.Count > 0)
				maxZoneNo = FiresecManager.Zones.Max(x => x.No);
			var zoneMap = new List<Guid>();
			FiresecManager.Zones.ForEach(zone => zoneMap.Add(zone.UID));
			foreach (var zone in deviceConfiguration.Zones)
				if (!zoneMap.Contains(zone.UID))
				{
					zone.No += maxZoneNo;
					FiresecManager.Zones.Add(zone);
				}
		}

		static void AddDeviceTree(Device device)
		{
			var hasDevice = FiresecManager.Devices.Any(x => x.GetId() == device.GetId());
			if (!hasDevice)
			{
				var existingParent = FiresecManager.Devices.FirstOrDefault(x => x.GetId() == device.Parent.GetId());
				if (existingParent != null)
				{
					existingParent.Children.Add(device);
				}
			}
		}

		static void MergePlans(PlansConfiguration plansConfiguration)
		{
			plansConfiguration.Update();
			foreach (var plan in plansConfiguration.Plans)
			{
				FiresecManager.PlansConfiguration.Plans.Add(plan);
				Trace.WriteLine(plan.Caption);
			}
		}
	}
}