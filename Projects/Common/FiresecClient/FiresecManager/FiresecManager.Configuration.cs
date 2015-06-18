﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Elements;
using GKProcessor;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static FiresecConfiguration FiresecConfiguration { get; set; }
		public static PlansConfiguration PlansConfiguration
		{
			get { return ConfigurationCash.PlansConfiguration; }
			set { ConfigurationCash.PlansConfiguration = value; }
		}

		public static SystemConfiguration SystemConfiguration { get; set; }
		public static SecurityConfiguration SecurityConfiguration { get; set; }
		public static LayoutsConfiguration LayoutsConfiguration { get; set; }

		public static void UpdateFiles()
		{
			try
			{
				FileHelper.Synchronize();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.UpdateFiles");
				LoadingErrorManager.Add(e);
			}
		}

		public static void CopyStream(Stream input, Stream output)
		{
			var buffer = new byte[8 * 1024];
			int length;
			while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, length);
			}
			output.Close();
		}

		public static void GetConfiguration(string configurationFolderName)
		{
			try
			{
				var serverConfigDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");
				var configDirectory = AppDataFolderHelper.GetLocalFolder(configurationFolderName);
				var contentDirectory = Path.Combine(configDirectory, "Content");

				if (Directory.Exists(configDirectory))
					Directory.Delete(configDirectory, true);
				Directory.CreateDirectory(configDirectory);
				Directory.CreateDirectory(contentDirectory);

				FiresecConfiguration = new FiresecConfiguration();
				if (ServiceFactoryBase.ContentService != null)
					ServiceFactoryBase.ContentService.Invalidate();

				if (ConnectionSettingsManager.IsRemote)
				{
					var configFileName = Path.Combine(configDirectory, "Config.fscp");
					var configFileStream = File.Create(configFileName);
					var stream = FiresecService.GetConfig();
					CopyStream(stream, configFileStream);
					LoadFromZipFile(configFileName);
				}
				else
				{
					foreach (var fileName in Directory.GetFiles(serverConfigDirectory))
					{
						var file = Path.GetFileName(fileName);
						File.Copy(fileName, Path.Combine(configDirectory, file), true);
					}
					foreach (var fileName in Directory.GetFiles(Path.Combine(serverConfigDirectory, "Content")))
					{
						var file = Path.GetFileName(fileName);
						File.Copy(fileName, Path.Combine(contentDirectory, file), true);
					}
					LoadConfigFromDirectory(configDirectory);
				}

				UpdateConfiguration();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.GetConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		public static void UpdateConfiguration()
		{
			try
			{
				if (LayoutsConfiguration == null)
					LayoutsConfiguration = new LayoutsConfiguration();
				LayoutsConfiguration.Update();
				PlansConfiguration.Update();
				SystemConfiguration.UpdateConfiguration();
				GKDriversCreator.Create();
				GKManager.UpdateConfiguration();
				GKManager.CreateStates();
				SKDManager.UpdateConfiguration();
				UpdatePlansConfiguration();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.UpdateConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		public static void UpdatePlansConfiguration()
		{
			try
			{
				GKManager.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.Doors.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });

				SKDManager.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				SKDManager.Zones.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				SKDManager.Doors.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });

				SystemConfiguration.Cameras.ForEach(x => x.PlanElementUIDs = new List<Guid>());
				SystemConfiguration.AutomationConfiguration.Procedures.ForEach(x => x.PlanElementUIDs = new List<Guid>());

				var gkDeviceMap = new Dictionary<Guid, GKDevice>();
				foreach (var device in GKManager.Devices)
				{
					if (!gkDeviceMap.ContainsKey(device.UID))
						gkDeviceMap.Add(device.UID, device);
				}
				var gkDoorMap = new Dictionary<Guid, GKDoor>();
				foreach (var door in GKManager.Doors)
				{
					if (!gkDoorMap.ContainsKey(door.UID))
						gkDoorMap.Add(door.UID, door);
				}

				var doorMap = new Dictionary<Guid, SKDDoor>();
				foreach (var door in SKDManager.SKDConfiguration.Doors)
				{
					if (!doorMap.ContainsKey(door.UID))
						doorMap.Add(door.UID, door);
				}
				var skdDeviceMap = new Dictionary<Guid, SKDDevice>();
				foreach (var skdDevice in SKDManager.Devices)
				{
					if (!skdDeviceMap.ContainsKey(skdDevice.UID))
						skdDeviceMap.Add(skdDevice.UID, skdDevice);
				}
				var skdZoneMap = new Dictionary<Guid, SKDZone>();
				foreach (var skdZone in SKDManager.Zones)
				{
					if (!skdZoneMap.ContainsKey(skdZone.UID))
						skdZoneMap.Add(skdZone.UID, skdZone);
				}

				var cameraMap = new Dictionary<Guid, Camera>();
				foreach (var camera in SystemConfiguration.Cameras)
				{
					if (!cameraMap.ContainsKey(camera.UID))
						cameraMap.Add(camera.UID, camera);
				}

				var procedureMap = new Dictionary<Guid, Procedure>();
				foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures)
				{
					if (!procedureMap.ContainsKey(procedure.Uid))
						procedureMap.Add(procedure.Uid, procedure);
				}

				var planMap = new Dictionary<Guid, Plan>();
				PlansConfiguration.AllPlans.ForEach(plan => planMap.Add(plan.UID, plan));
				foreach (var plan in PlansConfiguration.AllPlans)
				{
					for (int i = plan.ElementGKDevices.Count(); i > 0; i--)
					{
						var elementGKDevice = plan.ElementGKDevices[i - 1];
						elementGKDevice.UpdateZLayer();
						if (gkDeviceMap.ContainsKey(elementGKDevice.DeviceUID))
							gkDeviceMap[elementGKDevice.DeviceUID].PlanElementUIDs.Add(elementGKDevice.UID);
					}
					for (int i = plan.ElementRectangleGKSKDZones.Count(); i > 0; i--)
					{
						var elementRectangleGKSKDZone = plan.ElementRectangleGKSKDZones[i - 1];
						elementRectangleGKSKDZone.UpdateZLayer();
						if (gkDoorMap.ContainsKey(elementRectangleGKSKDZone.ZoneUID))
							gkDoorMap[elementRectangleGKSKDZone.ZoneUID].PlanElementUIDs.Add(elementRectangleGKSKDZone.UID);
					}
					for (int i = plan.ElementPolygonGKSKDZones.Count(); i > 0; i--)
					{
						var elementPolygonGKSKDZone = plan.ElementPolygonGKSKDZones[i - 1];
						elementPolygonGKSKDZone.UpdateZLayer();
						if (gkDoorMap.ContainsKey(elementPolygonGKSKDZone.ZoneUID))
							gkDoorMap[elementPolygonGKSKDZone.ZoneUID].PlanElementUIDs.Add(elementPolygonGKSKDZone.UID);
					}
					for (int i = plan.ElementGKDoors.Count(); i > 0; i--)
					{
						var elementGKDoor = plan.ElementGKDoors[i - 1];
						elementGKDoor.UpdateZLayer();
						if (gkDoorMap.ContainsKey(elementGKDoor.DoorUID))
							gkDoorMap[elementGKDoor.DoorUID].PlanElementUIDs.Add(elementGKDoor.UID);
					}

					for (int i = plan.ElementSKDDevices.Count(); i > 0; i--)
					{
						var elementSKDDevice = plan.ElementSKDDevices[i - 1];
						elementSKDDevice.UpdateZLayer();
						if (skdDeviceMap.ContainsKey(elementSKDDevice.DeviceUID))
							skdDeviceMap[elementSKDDevice.DeviceUID].PlanElementUIDs.Add(elementSKDDevice.UID);
					}
					for (int i = plan.ElementDoors.Count(); i > 0; i--)
					{
						var elementDoor = plan.ElementDoors[i - 1];
						elementDoor.UpdateZLayer();
						if (doorMap.ContainsKey(elementDoor.DoorUID))
							doorMap[elementDoor.DoorUID].PlanElementUIDs.Add(elementDoor.UID);
					}
					foreach (var skdZone in plan.ElementPolygonSKDZones)
					{
						UpdateSKDZoneType(skdZone, skdZone.ZoneUID != Guid.Empty && skdZoneMap.ContainsKey(skdZone.ZoneUID) ? skdZoneMap[skdZone.ZoneUID] : null);
						if (skdZoneMap.ContainsKey(skdZone.ZoneUID))
							skdZoneMap[skdZone.ZoneUID].PlanElementUIDs.Add(skdZone.UID);
					}
					foreach (var skdZone in plan.ElementRectangleSKDZones)
					{
						UpdateSKDZoneType(skdZone, skdZone.ZoneUID != Guid.Empty && skdZoneMap.ContainsKey(skdZone.ZoneUID) ? skdZoneMap[skdZone.ZoneUID] : null);
						if (skdZoneMap.ContainsKey(skdZone.ZoneUID))
							skdZoneMap[skdZone.ZoneUID].PlanElementUIDs.Add(skdZone.UID);
					}

					for (int i = plan.ElementExtensions.Count(); i > 0; i--)
					{
						var elementExtension = plan.ElementExtensions[i - 1];
						elementExtension.UpdateZLayer();
						var elementCamera = elementExtension as ElementCamera;
						if (elementCamera != null && cameraMap.ContainsKey(elementCamera.CameraUID))
							cameraMap[elementCamera.CameraUID].PlanElementUIDs.Add(elementExtension.UID);
						else if (elementExtension is ElementProcedure)
						{
							var elementProcedure = (ElementProcedure)elementExtension;
							if (procedureMap.ContainsKey(elementProcedure.ProcedureUID))
								procedureMap[elementProcedure.ProcedureUID].PlanElementUIDs.Add(elementExtension.UID);
						}
					}

					foreach (var subplan in plan.ElementSubPlans)
						UpdateSubPlan(subplan, subplan.PlanUID != Guid.Empty && planMap.ContainsKey(subplan.PlanUID) ? planMap[subplan.PlanUID] : null);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecManager.UpdatePlansConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		private static void UpdateSKDZoneType(IElementZone elementZone, SKDZone zone)
		{
			elementZone.SetZLayer(zone == null ? 50 : 60);
			elementZone.BackgroundColor = zone == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Green;
		}
		private static void UpdateSubPlan(ElementSubPlan elementSubPlan, Plan plan)
		{
			elementSubPlan.BackgroundColor = plan == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Green;
		}
	}
}