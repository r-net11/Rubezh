using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;
using Microsoft.Win32;

namespace FireAdministrator.ViewModels
{
	public class MergeConfigurationHelper
	{
		public PlansConfiguration PlansConfiguration;
		public SKDConfiguration SKDConfiguration;

		public void Merge()
		{
			var openDialog = new OpenFileDialog()
			{
				Filter = "firesec2 files|*.fscp",
				DefaultExt = "firesec2 files|*.fscp"
			};
			if (openDialog.ShowDialog().Value)
			{
				ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
				var folderName = AppDataFolderHelper.GetLocalFolder("Administrator/MergeConfiguration");
				var configFileName = Path.Combine(folderName, "Config.fscp");
				if (Directory.Exists(folderName))
					Directory.Delete(folderName, true);
				Directory.CreateDirectory(folderName);
				File.Copy(openDialog.FileName, configFileName);
				LoadFromZipFile(configFileName);
				ServiceFactory.ContentService.Clear();

				FiresecManager.UpdateConfiguration();
				SKDManager.UpdateConfiguration();

				ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
				ServiceFactory.Layout.Close();

				ServiceFactory.SaveService.PlansChanged = true;
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.Layout.ShowFooter(null);
			}
		}

		public void LoadFromZipFile(string fileName)
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
			var zipConfigurationItemsCollection = ZipSerializeHelper.DeSerialize<ZipConfigurationItemsCollection>(zipConfigurationItemsCollectionFileName, true);
			if (zipConfigurationItemsCollection == null)
			{
				Logger.Error("FiresecManager.LoadFromZipFile zipConfigurationItemsCollection == null");
				return;
			}

			foreach (var zipConfigurationItem in zipConfigurationItemsCollection.GetWellKnownZipConfigurationItems())
			{
				var configurationFileName = Path.Combine(unzipFolderPath, zipConfigurationItem.Name);
				if (File.Exists(configurationFileName))
				{
					switch (zipConfigurationItem.Name)
					{
						case "PlansConfiguration.xml":
							PlansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationFileName, true);
							break;

						case "SKDConfiguration.xml":
							SKDConfiguration = ZipSerializeHelper.DeSerialize<SKDConfiguration>(configurationFileName, true);
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

			MergeSKDConfiguration();
		}

		void MergeSKDConfiguration()
		{
			SKDConfiguration.Update();
			CreateNewSKDUIDs();
			SKDConfiguration.Update();

			foreach (var device in SKDConfiguration.Devices)
			{
				SKDManager.Devices.Add(device);
				if (device.Parent != null && device.Parent.Parent == null)
				{
					SKDManager.SKDConfiguration.RootDevice.Children.Add(device);
				}
			}
			foreach (var zone in SKDConfiguration.Zones)
			{
				SKDManager.Zones.Add(zone);
			}
			foreach (var door in SKDConfiguration.Doors)
			{
				SKDManager.Doors.Add(door);
			}

			ReorderNos(SKDManager.Zones);
			ReorderNos(SKDManager.Doors);
		}

		void CreateNewSKDUIDs()
		{
			foreach (var device in SKDConfiguration.Devices)
			{
				var uid = Guid.NewGuid();
				SKDDeviceUIDs.Add(device.UID, uid);
				device.UID = uid;
			}
			foreach (var zone in SKDConfiguration.Zones)
			{
				var uid = Guid.NewGuid();
				SKDZoneUIDs.Add(zone.UID, uid);
				zone.UID = uid;
			}
			foreach (var door in SKDConfiguration.Doors)
			{
				var uid = Guid.NewGuid();
				SKDDoorUIDs.Add(door.UID, uid);
				door.UID = uid;
			}

			foreach (var device in SKDConfiguration.Devices)
			{
				if (device.ZoneUID != Guid.Empty)
				{
					device.ZoneUID = ReplaceUID(device.ZoneUID, SKDZoneUIDs);
				}
			}

			foreach (var door in SKDConfiguration.Doors)
			{
				door.InDeviceUID = ReplaceUID(door.InDeviceUID, SKDDeviceUIDs);
				door.OutDeviceUID = ReplaceUID(door.OutDeviceUID, SKDDeviceUIDs);
			}

			foreach (var plan in PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementSKDDevices)
				{
					if (element.DeviceUID != Guid.Empty)
						element.DeviceUID = SKDDeviceUIDs[element.DeviceUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementRectangleSKDZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = SKDZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonSKDZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = SKDZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
			}
		}

		Dictionary<Guid, Guid> GKDeviceUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKDoorUIDs = new Dictionary<Guid, Guid>();

		Dictionary<Guid, Guid> SKDDeviceUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> SKDZoneUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> SKDDoorUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> PlenElementUIDs = new Dictionary<Guid, Guid>();

		Guid ReplaceUID(Guid uid, Dictionary<Guid, Guid> dictionary)
		{
			if (uid != Guid.Empty)
			{
				return dictionary[uid];
			}
			return Guid.Empty;
		}

		void ReorderNos<T>(List<T> models)
			where T : ModelBase
		{
			for (int i = 0; i < models.Count; i++)
			{
				models[i].No = i + 1;
			}
		}
	}
}