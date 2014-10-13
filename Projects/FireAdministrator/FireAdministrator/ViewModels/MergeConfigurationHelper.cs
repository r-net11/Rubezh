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
			GKDeviceConfiguration.Update();
			PlansConfiguration.Update();
			CreateNewGKUIDs();
			GKDeviceConfiguration.Update();
			PlansConfiguration.Update();
			var errors = new StringBuilder();

			var maxZoneNo = 0;
			if (GKManager.Zones.Count > 0)
				maxZoneNo = GKManager.Zones.Max(x=>x.No);

			var maxGuardZoneNo = 0;
			if (GKManager.GuardZones.Count > 0)
				maxGuardZoneNo = GKManager.GuardZones.Max(x => x.No);

			var maxDirectionNo = 0;
			if (GKManager.Directions.Count > 0)
				maxDirectionNo = GKManager.Directions.Max(x => x.No);

			var maxDoorNo = 0;
			if (GKManager.Doors.Count > 0)
				maxDoorNo = GKManager.Doors.Max(x => x.No);

			if (GKDeviceConfiguration == null)
				GKDeviceConfiguration = new GKDeviceConfiguration();
			if (PlansConfiguration == null)
				PlansConfiguration = new PlansConfiguration();

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
				zone.No = zone.No + maxZoneNo;
				GKManager.Zones.Add(zone);
			}
			foreach (var guardZone in GKDeviceConfiguration.GuardZones)
			{
				guardZone.No = guardZone.No + maxGuardZoneNo;
				GKManager.GuardZones.Add(guardZone);
			}
			foreach (var direction in GKDeviceConfiguration.Directions)
			{
				direction.No = direction.No + maxDirectionNo;
				GKManager.Directions.Add(direction);
			}
			foreach (var door in GKDeviceConfiguration.Doors)
			{
				door.No = door.No + maxDoorNo;
				GKManager.Doors.Add(door);
			}
			foreach (var code in GKDeviceConfiguration.Codes)
			{
				GKManager.DeviceConfiguration.Codes.Add(code);
			}
			foreach (var schedules in GKDeviceConfiguration.Schedules)
			{
				GKManager.DeviceConfiguration.Schedules.Add(schedules);
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
				MessageBoxService.ShowError(errorsString, "В результате слияния конфигурации возникли ошибки");
			}
		}

		void MergeSKDConfiguration()
		{
			SKDConfiguration.Update();
			CreateNewSKDUIDs();
			SKDConfiguration.Update();

			var maxZoneNo = 0;
			if (SKDManager.Zones.Count > 0)
				maxZoneNo = SKDManager.Zones.Max(x => x.No);

			var maxDoorNo = 0;
			if (SKDManager.Doors.Count > 0)
				maxDoorNo = SKDManager.Doors.Max(x => x.No);


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
				zone.No = zone.No + maxZoneNo;
				SKDManager.Zones.Add(zone);
			}
			foreach (var door in SKDConfiguration.Doors)
			{
				door.No = door.No + maxDoorNo;
				SKDManager.Doors.Add(door);
			}
		}

		void CreateNewGKUIDs()
		{
			foreach (var device in GKDeviceConfiguration.Devices)
			{
				var uid = Guid.NewGuid();
				GKDeviceUIDs.Add(device.UID, uid);
				device.UID = uid;
			}
			foreach (var zone in GKDeviceConfiguration.Zones)
			{
				var uid = Guid.NewGuid();
				GKZoneUIDs.Add(zone.UID, uid);
				zone.UID = uid;
			}
			foreach (var guardZone in GKDeviceConfiguration.GuardZones)
			{
				var uid = Guid.NewGuid();
				GKGuardZoneUIDs.Add(guardZone.UID, uid);
				guardZone.UID = uid;
			}
			foreach (var direction in GKDeviceConfiguration.Directions)
			{
				var uid = Guid.NewGuid();
				GKDirectionUIDs.Add(direction.UID, uid);
				direction.UID = uid;
			}
			foreach (var door in GKDeviceConfiguration.Doors)
			{
				var uid = Guid.NewGuid();
				GKDoorUIDs.Add(door.UID, uid);
				door.UID = uid;
			}
			foreach (var code in GKDeviceConfiguration.Codes)
			{
				var uid = Guid.NewGuid();
				GKCodeUIDs.Add(code.UID, uid);
				code.UID = uid;
			}
			foreach (var schedule in GKDeviceConfiguration.Schedules)
			{
				var uid = Guid.NewGuid();
				GKScheduleUIDs.Add(schedule.UID, uid);
				schedule.UID = uid;
			}
			foreach (var delay in GKDeviceConfiguration.Delays)
			{
				var uid = Guid.NewGuid();
				GKDelayUIDs.Add(delay.UID, uid);
				delay.UID = uid;
			}

			foreach (var device in GKDeviceConfiguration.Devices)
			{
				for (int i = 0; i < device.ZoneUIDs.Count; i++)
				{
					var zoneUID = device.ZoneUIDs[i];
					device.ZoneUIDs[i] = GKZoneUIDs[zoneUID];
				}
				ReplaceDeviceLogic(device.DeviceLogic);
				ReplaceDeviceLogic(device.NSLogic);
			}

			foreach (var direction in GKDeviceConfiguration.Directions)
			{
				foreach (var directionZone in direction.DirectionZones)
				{
					var zoneUID = directionZone.ZoneUID;
					directionZone.ZoneUID = GKZoneUIDs[zoneUID];
				}
				foreach (var directionDevice in direction.DirectionDevices)
				{
					var deviceUID = directionDevice.DeviceUID;
					directionDevice.DeviceUID = GKDeviceUIDs[deviceUID];
				}
			}

			foreach (var pumpStation in GKDeviceConfiguration.PumpStations)
			{
				for (int i = 0; i < pumpStation.NSDeviceUIDs.Count; i++)
				{
					var deviceUID = pumpStation.NSDeviceUIDs[i];
					pumpStation.NSDeviceUIDs[i] = GKDeviceUIDs[deviceUID];
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
						element.DeviceUID = GKDeviceUIDs[element.DeviceUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementRectangleGKZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = GKZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonGKZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = GKZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementRectangleGKGuardZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = GKGuardZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonGKGuardZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = GKGuardZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementRectangleGKDirections)
				{
					if (element.DirectionUID != Guid.Empty)
						element.DirectionUID = GKDirectionUIDs[element.DirectionUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonGKDirections)
				{
					if (element.DirectionUID != Guid.Empty)
						element.DirectionUID = GKDirectionUIDs[element.DirectionUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementGKDoors)
				{
					if (element.DoorUID != Guid.Empty)
						element.DoorUID = GKDeviceUIDs[element.DoorUID];
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
					clause.ZoneUIDs[i] = GKZoneUIDs[zoneUID];
				}
				for (int i = 0; i < clause.GuardZoneUIDs.Count; i++)
				{
					var guardZoneUID = clause.GuardZoneUIDs[i];
					clause.GuardZoneUIDs[i] = GKGuardZoneUIDs[guardZoneUID];
				}
				for (int i = 0; i < clause.DeviceUIDs.Count; i++)
				{
					var deviceUID = clause.DeviceUIDs[i];
					clause.DeviceUIDs[i] = GKDeviceUIDs[deviceUID];
				}
				for (int i = 0; i < clause.DirectionUIDs.Count; i++)
				{
					var directionUID = clause.DirectionUIDs[i];
					clause.DirectionUIDs[i] = GKDirectionUIDs[directionUID];
				}
			}
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
					var zoneUID = device.ZoneUID;
					device.ZoneUID = SKDZoneUIDs[zoneUID];
				}
			}

			foreach (var door in SKDConfiguration.Doors)
			{
				if (door.InDeviceUID != Guid.Empty)
				{
					var inDeviceUID = door.InDeviceUID;
					door.InDeviceUID = SKDDeviceUIDs[inDeviceUID];
				}
				if (door.OutDeviceUID != Guid.Empty)
				{
					var outDeviceUID = door.OutDeviceUID;
					door.OutDeviceUID = SKDDeviceUIDs[outDeviceUID];
				}
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
				foreach (var element in plan.ElementDevices)
				{
					if (element.UID != Guid.Empty)
						element.UID = SKDDoorUIDs[element.UID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
			}
		}

		Dictionary<Guid, Guid> GKDeviceUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKZoneUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKGuardZoneUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKDirectionUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKDoorUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKCodeUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKScheduleUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKDelayUIDs = new Dictionary<Guid, Guid>();

		Dictionary<Guid, Guid> SKDDeviceUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> SKDZoneUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> SKDDoorUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> PlenElementUIDs = new Dictionary<Guid, Guid>();
	}
}