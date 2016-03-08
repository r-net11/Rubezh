using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;
using Microsoft.Win32;
using RubezhAPI;
using Infrustructure.Plans.Interfaces;

namespace FireAdministrator.ViewModels
{
	public class MergeConfigurationHelper
	{
		public PlansConfiguration PlansConfiguration;
		public GKDeviceConfiguration GKDeviceConfiguration;

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
				ServiceFactory.ContentService.Invalidate();

				ClientManager.UpdateConfiguration();
				GKManager.UpdateConfiguration();

				ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
				ServiceFactory.Layout.Close();
				if (ApplicationService.Modules.Any(x => x.Name == "Групповой контроллер"))
					ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(Guid.Empty);

				ServiceFactory.SaveService.PlansChanged = true;
				ServiceFactory.SaveService.GKChanged = true;
				ServiceFactory.Layout.ShowFooter(null);
			}
		}

		public void LoadFromZipFile(string fileName)
		{
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var fileInfo = new FileInfo(fileName);
			var unzipFolderPath = fileInfo.Directory.FullName;
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

			foreach (var zipConfigurationItem in zipConfigurationItemsCollection.GetWellKnownZipConfigurationItems())
			{
				var configurationFileName = Path.Combine(unzipFolderPath, zipConfigurationItem.Name);
				if (File.Exists(configurationFileName))
				{
					switch (zipConfigurationItem.Name)
					{
						case "PlansConfiguration.xml":
							PlansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationFileName);
							break;

						case "GKDeviceConfiguration.xml":
							GKDeviceConfiguration = ZipSerializeHelper.DeSerialize<GKDeviceConfiguration>(configurationFileName);
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
		}

		void MergeGKDeviceConfiguration()
		{
			GKDeviceConfiguration.Update();
			PlansConfiguration.Update();
			CreateNewGKUIDs();
			GKDeviceConfiguration.Update();
			PlansConfiguration.Update();
			var errors = new StringBuilder();

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
						if (driver.DriverType == GKDriverType.RSR2_KAU)
						{
							var existingKAUDevice = existingGKDevice.Children.FirstOrDefault(x => x.Driver != null && x.DriverType == GKDriverType.RSR2_KAU && x.IntAddress == device.IntAddress);
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
				GKManager.Zones.Add(zone);
			}
			foreach (var guardZone in GKDeviceConfiguration.GuardZones)
			{
				GKManager.GuardZones.Add(guardZone);
			}
			foreach (var direction in GKDeviceConfiguration.Directions)
			{
				GKManager.Directions.Add(direction);
			}
			foreach (var mpt in GKDeviceConfiguration.MPTs)
			{
				GKManager.MPTs.Add(mpt);
			}
			foreach (var delay in GKDeviceConfiguration.Delays)
			{
				GKManager.DeviceConfiguration.Delays.Add(delay);
			}
			foreach (var pumpStation in GKDeviceConfiguration.PumpStations)
			{
				GKManager.DeviceConfiguration.PumpStations.Add(pumpStation);
			}
			foreach (var mpt in GKDeviceConfiguration.MPTs)
			{
				GKManager.DeviceConfiguration.MPTs.Add(mpt);
			}
			foreach (var door in GKDeviceConfiguration.Doors)
			{
				GKManager.Doors.Add(door);
			}
			foreach (var code in GKDeviceConfiguration.Codes)
			{
				GKManager.DeviceConfiguration.Codes.Add(code);
			}
			//foreach (var schedules in GKDeviceConfiguration.Schedules)
			//{
			//    GKManager.DeviceConfiguration.Schedules.Add(schedules);
			//}

			ReorderNos(GKManager.Zones);
			ReorderNos(GKManager.GuardZones);
			ReorderNos(GKManager.GuardZones);
			ReorderNos(GKManager.PumpStations);
			ReorderNos(GKManager.MPTs);
			ReorderNos(GKManager.Doors);
			ReorderNos(GKManager.DeviceConfiguration.Codes);
			//ReorderNos(GKManager.DeviceConfiguration.Schedules);

			foreach (var plan in PlansConfiguration.Plans)
			{
				ClientManager.PlansConfiguration.Plans.Add(plan);
			}

			GKManager.UpdateConfiguration();
			ClientManager.UpdateConfiguration();

			var errorsString = errors.ToString();
			if (!string.IsNullOrEmpty(errorsString))
			{
				MessageBoxService.ShowError(errorsString, "В результате слияния конфигурации возникли ошибки");
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
			foreach (var delay in GKDeviceConfiguration.Delays)
			{
				var uid = Guid.NewGuid();
				GKDelayUIDs.Add(delay.UID, uid);
				delay.UID = uid;
			}
			foreach (var direction in GKDeviceConfiguration.Directions)
			{
				var uid = Guid.NewGuid();
				GKDirectionUIDs.Add(direction.UID, uid);
				direction.UID = uid;
			}
			foreach (var mpt in GKDeviceConfiguration.MPTs)
			{
				var uid = Guid.NewGuid();
				GKMPTUIDs.Add(mpt.UID, uid);
				mpt.UID = uid;
			}
			foreach (var pumpStation in GKDeviceConfiguration.PumpStations)
			{
				var uid = Guid.NewGuid();
				GKPumpStationUIDs.Add(pumpStation.UID, uid);
				pumpStation.UID = uid;
			}
			foreach (var mpt in GKDeviceConfiguration.MPTs)
			{
				var uid = Guid.NewGuid();
				GKMPTUIDs.Add(mpt.UID, uid);
				mpt.UID = uid;
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
			//foreach (var schedule in GKDeviceConfiguration.Schedules)
			//{
			//    var uid = Guid.NewGuid();
			//    GKScheduleUIDs.Add(schedule.UID, uid);
			//    schedule.UID = uid;
			//}

			foreach (var device in GKDeviceConfiguration.Devices)
			{
				for (int i = 0; i < device.ZoneUIDs.Count; i++)
				{
					device.ZoneUIDs[i] = ReplaceUID(device.ZoneUIDs[i], GKZoneUIDs);
				}
				ReplaceLogic(device.Logic);
				ReplaceLogic(device.NSLogic);
			}

			foreach (var delay in GKDeviceConfiguration.Delays)
			{
				delay.PumpStationUID = ReplaceUID(delay.PumpStationUID, GKPumpStationUIDs);
				ReplaceLogic(delay.Logic);
			}

			foreach (var direction in GKDeviceConfiguration.Directions)
			{
				ReplaceLogic(direction.Logic);
			}

			foreach (var mpt in GKDeviceConfiguration.MPTs)
			{
				ReplaceLogic(mpt.MptLogic.OnClausesGroup);
				ReplaceLogic(mpt.MptLogic.OffClausesGroup);
				ReplaceLogic(mpt.MptLogic.StopClausesGroup);
			}

			foreach (var guardZone in GKDeviceConfiguration.GuardZones)
			{
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					guardZoneDevice.DeviceUID = ReplaceUID(guardZoneDevice.DeviceUID, GKDeviceUIDs);
					for (int i = 0; i < guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs.Count; i++)
					{
						guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs[i] = ReplaceUID(guardZoneDevice.CodeReaderSettings.AlarmSettings.CodeUIDs[i], GKCodeUIDs);
					}
					for (int i = 0; i < guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs.Count; i++)
					{
						guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs[i] = ReplaceUID(guardZoneDevice.CodeReaderSettings.ResetGuardSettings.CodeUIDs[i], GKCodeUIDs);
					}
					for (int i = 0; i < guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs.Count; i++)
					{
						guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs[i] = ReplaceUID(guardZoneDevice.CodeReaderSettings.SetGuardSettings.CodeUIDs[i], GKCodeUIDs);
					}
				}
			}

			foreach (var pumpStation in GKDeviceConfiguration.PumpStations)
			{
				for (int i = 0; i < pumpStation.NSDeviceUIDs.Count; i++)
				{
					pumpStation.NSDeviceUIDs[i] = ReplaceUID(pumpStation.NSDeviceUIDs[i], GKDeviceUIDs);
				}
				ReplaceLogic(pumpStation.StartLogic);
				ReplaceLogic(pumpStation.AutomaticOffLogic);
				ReplaceLogic(pumpStation.StopLogic);
			}

			foreach (var mpt in GKDeviceConfiguration.MPTs)
			{
				foreach (var mptDevice in mpt.MPTDevices)
				{
					mptDevice.DeviceUID = ReplaceUID(mptDevice.DeviceUID, GKDeviceUIDs);
				}
				ReplaceLogic(mpt.MptLogic.OnClausesGroup);
				ReplaceLogic(mpt.MptLogic.OffClausesGroup);
				ReplaceLogic(mpt.MptLogic.StopClausesGroup);
			}

			foreach (var door in GKDeviceConfiguration.Doors)
			{
				door.EnterDeviceUID = ReplaceUID(door.EnterDeviceUID, GKDeviceUIDs);
				door.ExitDeviceUID = ReplaceUID(door.ExitDeviceUID, GKDeviceUIDs);
				door.EnterButtonUID = ReplaceUID(door.EnterButtonUID, GKDeviceUIDs);
				door.ExitButtonUID = ReplaceUID(door.ExitButtonUID, GKDeviceUIDs);
				door.LockDeviceUID = ReplaceUID(door.LockDeviceUID, GKDeviceUIDs);
				door.LockDeviceExitUID = ReplaceUID(door.LockDeviceExitUID, GKDeviceUIDs);
				door.LockControlDeviceUID = ReplaceUID(door.LockControlDeviceUID, GKDeviceUIDs);
				door.LockControlDeviceExitUID = ReplaceUID(door.LockControlDeviceExitUID, GKDeviceUIDs);
				ReplaceLogic(door.OpenRegimeLogic);
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
				foreach (var element in plan.ElementRectangleGKDelays)
				{
					if (element.DelayUID != Guid.Empty)
						element.DelayUID = GKDelayUIDs[element.DelayUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonGKDelays)
				{
					if (element.DelayUID != Guid.Empty)
						element.DelayUID = GKDelayUIDs[element.DelayUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in new IElementReference[0]
					.Union(plan.ElementRectangleGKPumpStations)
					.Union(plan.ElementPolygonSubPlans))
				{
					if (element.ItemUID != Guid.Empty)
						element.ItemUID = GKPumpStationUIDs[element.ItemUID];
					var uid = new Guid();
					PlenElementUIDs.Add(element.ItemUID, uid);
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
				foreach (var element in plan.ElementRectangleGKMPTs)
				{
					if (element.MPTUID != Guid.Empty)
						element.MPTUID = GKMPTUIDs[element.MPTUID];
					var uid = Guid.NewGuid();
					PlenElementUIDs.Add(element.UID, uid);
					element.UID = uid;
				}
				foreach (var element in plan.ElementPolygonGKMPTs)
				{
					if (element.MPTUID != Guid.Empty)
						element.MPTUID = GKMPTUIDs[element.MPTUID];
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
				if (device.PlanElementUIDs != null)
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

		void ReplaceLogic(GKLogic logic)
		{
			foreach (var clause in logic.OnClausesGroup.Clauses)
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
				for (int i = 0; i < clause.MPTUIDs.Count; i++)
				{
					var mptUID = clause.MPTUIDs[i];
					clause.MPTUIDs[i] = GKMPTUIDs[mptUID];
				}
				for (int i = 0; i < clause.DoorUIDs.Count; i++)
				{
					var doorUID = clause.DoorUIDs[i];
					clause.DoorUIDs[i] = GKDoorUIDs[doorUID];
				}
			}
		}

		void ReplaceLogic(GKClauseGroup gkClauseGroup)
		{
			foreach (var clause in gkClauseGroup.Clauses)
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
				for (int i = 0; i < clause.MPTUIDs.Count; i++)
				{
					var mptUID = clause.MPTUIDs[i];
					clause.MPTUIDs[i] = GKMPTUIDs[mptUID];
				}
				for (int i = 0; i < clause.DoorUIDs.Count; i++)
				{
					var doorUID = clause.DoorUIDs[i];
					clause.DoorUIDs[i] = GKDoorUIDs[doorUID];
				}
			}
		}

		Dictionary<Guid, Guid> GKDeviceUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKZoneUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKGuardZoneUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKDirectionUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKDelayUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKPumpStationUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKMPTUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKDoorUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKCodeUIDs = new Dictionary<Guid, Guid>();
		Dictionary<Guid, Guid> GKScheduleUIDs = new Dictionary<Guid, Guid>();

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