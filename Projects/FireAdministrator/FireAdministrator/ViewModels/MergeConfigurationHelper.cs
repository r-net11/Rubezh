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

namespace FireAdministrator.ViewModels
{
	public class MergeConfigurationHelper
	{
		public PlansConfiguration PlansConfiguration;
		public XDeviceConfiguration XDeviceConfiguration;

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
						case "XDeviceConfiguration.xml":
							XDeviceConfiguration = ZipSerializeHelper.DeSerialize<XDeviceConfiguration>(configurationFileName);
							break;

						case "PlansConfiguration.xml":
							PlansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationFileName);
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

			MergeXDeviceConfiguration();
		}

		void MergeXDeviceConfiguration()
		{
			var errors = new StringBuilder();
			var maxZoneNo = 0;
			if (XManager.Zones.Count > 0)
				maxZoneNo = XManager.Zones.Max(x=>x.No);

			var maxDirectionNo = 0;
			if (XManager.Directions.Count > 0)
				maxDirectionNo = XManager.Directions.Max(x => x.No);

			if (XDeviceConfiguration == null)
				XDeviceConfiguration = new XDeviceConfiguration();
			if (PlansConfiguration == null)
				PlansConfiguration = new PlansConfiguration();

			XDeviceConfiguration.Update();
			PlansConfiguration.Update();
			CreateNewUIDs();
			XDeviceConfiguration.Update();
			PlansConfiguration.Update();

			foreach (var gkDevice in XDeviceConfiguration.RootDevice.Children)
			{
				var ipAddress = "";
				var ipProperty = gkDevice.Properties.FirstOrDefault(x => x.Name == "IPAddress");
				if (ipProperty != null)
				{
					ipAddress = ipProperty.StringValue;
				}
				var existingGKDevice = XManager.DeviceConfiguration.RootDevice.Children.FirstOrDefault(x => x.Address == ipAddress);
				if (existingGKDevice != null)
				{
					foreach (var device in gkDevice.Children)
					{
						var driver = XManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
						if (driver.DriverType == XDriverType.KAU || driver.DriverType == XDriverType.RSR2_KAU)
						{
							var existingKAUDevice = existingGKDevice.Children.FirstOrDefault(x => x.Driver != null && (x.DriverType == XDriverType.KAU || x.DriverType == XDriverType.RSR2_KAU) && x.IntAddress == device.IntAddress);
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
					XManager.DeviceConfiguration.RootDevice.Children.Add(gkDevice);
				}
			}
			foreach (var zone in XDeviceConfiguration.Zones)
			{
				zone.No = (ushort)(zone.No + maxZoneNo);
				XManager.Zones.Add(zone);
			}
			foreach (var direction in XDeviceConfiguration.Directions)
			{
				direction.No = (ushort)(direction.No + maxDirectionNo);
				XManager.Directions.Add(direction);
			}

			foreach (var plan in PlansConfiguration.Plans)
			{
				FiresecManager.PlansConfiguration.Plans.Add(plan);
			}

			XManager.UpdateConfiguration();
			FiresecManager.UpdateConfiguration();

			var errorsString = errors.ToString();
			if (!string.IsNullOrEmpty(errorsString))
			{
				MessageBoxService.ShowError(errorsString, "В результате слияния конфигурации возникли ошибки");
			}
		}

		void CreateNewUIDs()
		{
			foreach (var device in XDeviceConfiguration.Devices)
			{
				var uid = Guid.NewGuid();
				DeviceUIDs.Add(device.BaseUID, uid);
				device.BaseUID = uid;
			}
			foreach (var zone in XDeviceConfiguration.Zones)
			{
				var uid = Guid.NewGuid();
				ZoneUIDs.Add(zone.BaseUID, uid);
				zone.BaseUID = uid;
			}
			foreach (var zone in XDeviceConfiguration.GuardZones)
			{
				var uid = Guid.NewGuid();
				GuardZoneUIDs.Add(zone.BaseUID, uid);
				zone.BaseUID = uid;
			}
			foreach (var direction in XDeviceConfiguration.Directions)
			{
				var uid = Guid.NewGuid();
				DirectionUIDs.Add(direction.BaseUID, uid);
				direction.BaseUID = uid;
			}

			foreach (var device in XDeviceConfiguration.Devices)
			{
				for (int i = 0; i < device.ZoneUIDs.Count; i++)
				{
					var zoneUID = device.ZoneUIDs[i];
					device.ZoneUIDs[i] = ZoneUIDs[zoneUID];
				}
				ReplaceDeviceLogic(device.DeviceLogic);
				ReplaceDeviceLogic(device.NSLogic);
			}

			foreach (var direction in XDeviceConfiguration.Directions)
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

			foreach (var pumpStation in XDeviceConfiguration.PumpStations)
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
				foreach (var element in plan.ElementXDevices)
				{
					if (element.XDeviceUID != Guid.Empty)
						element.XDeviceUID = DeviceUIDs[element.XDeviceUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementRectangleXZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = ZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonXZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = ZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementRectangleXGuardZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = GuardZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonXGuardZones)
				{
					if (element.ZoneUID != Guid.Empty)
						element.ZoneUID = GuardZoneUIDs[element.ZoneUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementRectangleXDirections)
				{
					if (element.DirectionUID != Guid.Empty)
						element.DirectionUID = DirectionUIDs[element.DirectionUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonXDirections)
				{
					if (element.DirectionUID != Guid.Empty)
						element.DirectionUID = DirectionUIDs[element.DirectionUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
			}

			foreach (var device in XDeviceConfiguration.Devices)
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

		void ReplaceDeviceLogic(XDeviceLogic deviceLogic)
		{
			foreach (var clause in deviceLogic.ClausesGroup.Clauses)
			{
				for (int i = 0; i < clause.ZoneUIDs.Count; i++)
				{
					var zoneUID = clause.ZoneUIDs[i];
					clause.ZoneUIDs[i] = ZoneUIDs[zoneUID];
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