using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;
using Microsoft.Win32;
using FiresecAPI.SKD;

namespace FireAdministrator.ViewModels
{
	public class MergeConfigurationHelper
	{
		public PlansConfiguration PlansConfiguration;
		public GKDeviceConfiguration GKDeviceConfiguration;
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
				//ZipConfigActualizeHelper.Actualize(openDialog.FileName, false);
				var folderName = AppDataFolderHelper.GetLocalFolder("Administrator/MergeConfiguration");
				var configFileName = Path.Combine(folderName, "Config.fscp");
				if (Directory.Exists(folderName))
					Directory.Delete(folderName, true);
				Directory.CreateDirectory(folderName);
				File.Copy(openDialog.FileName, configFileName);
				LoadFromZipFile(configFileName);
				ServiceFactory.ContentService.Invalidate();

				FiresecManager.UpdateConfiguration();
				GKManager.UpdateConfiguration();
				SKDManager.UpdateConfiguration();

				ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
				ServiceFactory.Layout.Close();
				if (ApplicationService.Modules.Any(x => x.Name == "Групповой контроллер"))
					ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Guid.Empty);

				ServiceFactory.SaveService.PlansChanged = true;
				ServiceFactory.SaveService.GKChanged = true;
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

			foreach (var zipConfigurationItem in zipConfigurationItemsCollection.GetWellKnownZipConfigurationItems)
			{
				var configurationFileName = Path.Combine(unzipFolderPath, zipConfigurationItem.Name);
				if (File.Exists(configurationFileName))
				{
					switch (zipConfigurationItem.Name)
					{
						case "PlansConfiguration.xml":
							PlansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationFileName, true);
							break;

						case "GKDeviceConfiguration.xml":
							GKDeviceConfiguration = ZipSerializeHelper.DeSerialize<GKDeviceConfiguration>(configurationFileName, true);
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

			MergeGKDeviceConfiguration();
			MergeSKDConfiguration();
		}

		void MergeGKDeviceConfiguration()
		{
			var errors = new StringBuilder();
			var maxZoneNo = 0;
			if (GKManager.Zones.Count > 0)
				maxZoneNo = GKManager.Zones.Max(x=>x.No);

			var maxGuardZoneNo = 0;
			if (GKManager.GuardZones.Count > 0)
				maxZoneNo = GKManager.GuardZones.Max(x => x.No);

			var maxDirectionNo = 0;
			if (GKManager.Directions.Count > 0)
				maxDirectionNo = GKManager.Directions.Max(x => x.No);

			if (GKDeviceConfiguration == null)
				GKDeviceConfiguration = new GKDeviceConfiguration();
			if (PlansConfiguration == null)
				PlansConfiguration = new PlansConfiguration();

			GKDeviceConfiguration.Update();
			PlansConfiguration.Update();
			CreateNewUIDs();
			GKDeviceConfiguration.Update();
			PlansConfiguration.Update();

			foreach (var gkControllerDevice in GKDeviceConfiguration.RootDevice.Children)
			{
				var ipAddress = "";
				var ipProperty = gkControllerDevice.Properties.FirstOrDefault(x => x.Name == "IPAddress");
				if (ipProperty != null)
				{
					ipAddress = ipProperty.StringValue;
				}
				var existingGKDevice = GKManager.DeviceConfiguration.RootDevice.Children.FirstOrDefault(x => x.Address == ipAddress);
				if (existingGKDevice != null)
				{
					foreach (var device in gkControllerDevice.Children)
					{
						var driver = GKManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
						if (driver.DriverType == GKDriverType.KAU || driver.DriverType == GKDriverType.RSR2_KAU)
						{
							var existingKAUDevice = existingGKDevice.Children.FirstOrDefault(x => x.Driver != null && (x.DriverType == GKDriverType.KAU || x.DriverType == GKDriverType.RSR2_KAU) && x.IntAddress == device.IntAddress);
							if (existingKAUDevice == null)
							{
								existingGKDevice.Children.Add(device);
							}
							else
							{
								errors.AppendLine("Устройство " + existingKAUDevice.PresentationName + " не было добавленно в конфигурацию из за совпадения адресов");
							}
						}
					}
				}
				else
				{
					GKManager.DeviceConfiguration.RootDevice.Children.Add(gkControllerDevice);
				}
			}
			foreach (var zone in GKDeviceConfiguration.Zones)
			{
				zone.No = (ushort)(zone.No + maxZoneNo);
				GKManager.Zones.Add(zone);
			}
			foreach (var guardZone in GKDeviceConfiguration.GuardZones)
			{
				guardZone.No = (ushort)(guardZone.No + maxGuardZoneNo);
				GKManager.GuardZones.Add(guardZone);
			}
			foreach (var direction in GKDeviceConfiguration.Directions)
			{
				direction.No = (ushort)(direction.No + maxDirectionNo);
				GKManager.Directions.Add(direction);
			}

			foreach (var plan in PlansConfiguration.Plans)
			{
				FiresecManager.PlansConfiguration.Plans.Add(plan);
			}

			GKManager.UpdateConfiguration();
			FiresecManager.UpdateConfiguration();
			SKDManager.UpdateConfiguration();

			var errorsString = errors.ToString();
			if (!string.IsNullOrEmpty(errorsString))
			{
				MessageBoxService.ShowErrorExtended(errorsString, "В результате слияния конфигурации возникли ошибки");
			}
		}

		void MergeSKDConfiguration()
		{

		}

		void CreateNewUIDs()
		{
			foreach (var device in GKDeviceConfiguration.Devices)
			{
				var uid = Guid.NewGuid();
				DeviceUIDs.Add(device.UID, uid);
				device.UID = uid;
			}
			foreach (var zone in GKDeviceConfiguration.Zones)
			{
				var uid = Guid.NewGuid();
				ZoneUIDs.Add(zone.UID, uid);
				zone.UID = uid;
			}
			foreach (var guardZone in GKDeviceConfiguration.GuardZones)
			{
				var uid = Guid.NewGuid();
				GuardZoneUIDs.Add(guardZone.UID, uid);
				guardZone.UID = uid;
			}
			foreach (var direction in GKDeviceConfiguration.Directions)
			{
				var uid = Guid.NewGuid();
				DirectionUIDs.Add(direction.UID, uid);
				direction.UID = uid;
			}

			foreach (var device in GKDeviceConfiguration.Devices)
			{
				for (int i = 0; i < device.ZoneUIDs.Count; i++)
				{
					var zoneUID = device.ZoneUIDs[i];
					device.ZoneUIDs[i] = ZoneUIDs[zoneUID];
				}
				ReplaceDeviceLogic(device.DeviceLogic);
				ReplaceDeviceLogic(device.NSLogic);
			}

			foreach (var direction in GKDeviceConfiguration.Directions)
			{
				foreach (var directionZone in direction.DirectionZones)
				{
					var zoneUID = directionZone.ZoneUID;
					directionZone.ZoneUID = ZoneUIDs[zoneUID];
				}
				foreach (var directionDevice in direction.DirectionDevices)
				{
					var deviceUID = directionDevice.DeviceUID;
					directionDevice.DeviceUID = DeviceUIDs[deviceUID];
				}
			}

			foreach (var pumpStation in GKDeviceConfiguration.PumpStations)
			{
				for (int i = 0; i < pumpStation.NSDeviceUIDs.Count; i++)
				{
					var deviceUID = pumpStation.NSDeviceUIDs[i];
					pumpStation.NSDeviceUIDs[i] = DeviceUIDs[deviceUID];
				}
				ReplaceDeviceLogic(pumpStation.StartLogic);
				ReplaceDeviceLogic(pumpStation.AutomaticOffLogic);
				ReplaceDeviceLogic(pumpStation.StopLogic);
			}

			foreach (var plan in PlansConfiguration.AllPlans)
			{
				foreach (var element in plan.ElementGKDevices)
				{
					if (element.DeviceUID != Guid.Empty)
						element.DeviceUID = DeviceUIDs[element.DeviceUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementRectangleGKZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = ZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonGKZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = ZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementRectangleGKGuardZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = GuardZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonGKGuardZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = GuardZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementRectangleGKDirections)
				{
					if (element.DirectionUID != Guid.Empty)
						element.DirectionUID = DirectionUIDs[element.DirectionUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonGKDirections)
				{
					if (element.DirectionUID != Guid.Empty)
						element.DirectionUID = DirectionUIDs[element.DirectionUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
			}

			foreach (var device in GKDeviceConfiguration.Devices)
			{
				var uids = new List<Guid>();
				foreach (var planElementUID in device.PlanElementUIDs)
				{
					uids.Add(PlenElementUIDs[planElementUID]);
				}
				device.PlanElementUIDs = uids;
			}

			foreach (var plan in PlansConfiguration.AllPlans)
			{
				plan.UID = Guid.NewGuid();
			}
		}

		void ReplaceDeviceLogic(GKDeviceLogic deviceLogic)
		{
			foreach (var clause in deviceLogic.ClausesGroup.Clauses)
			{
				for (int i = 0; i < clause.ZoneUIDs.Count; i++)
				{
					var zoneUID = clause.ZoneUIDs[i];
					clause.ZoneUIDs[i] = ZoneUIDs[zoneUID];
				}
				for (int i = 0; i < clause.GuardZoneUIDs.Count; i++)
				{
					var guardZoneUID = clause.GuardZoneUIDs[i];
					clause.GuardZoneUIDs[i] = GuardZoneUIDs[guardZoneUID];
				}
				for (int i = 0; i < clause.DeviceUIDs.Count; i++)
				{
					var deviceUID = clause.DeviceUIDs[i];
					clause.DeviceUIDs[i] = DeviceUIDs[deviceUID];
				}
				for (int i = 0; i < clause.DirectionUIDs.Count; i++)
				{
					var directionUID = clause.DirectionUIDs[i];
					clause.DirectionUIDs[i] = DirectionUIDs[directionUID];
				}
			}
		}

		Dictionary<Guid, Guid> DeviceUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> ZoneUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GuardZoneUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> DirectionUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> PlenElementUIDs = new Dictionary<Guid, Guid>();
	}
}