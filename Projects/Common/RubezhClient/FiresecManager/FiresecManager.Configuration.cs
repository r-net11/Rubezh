using Common;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Elements;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrustructure.Plans.Interfaces;

namespace RubezhClient
{
	public partial class ClientManager
	{
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
				Logger.Error(e, "ClientManager.UpdateFiles");
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
				{
					Directory.Delete(configDirectory, true);
				}
				Directory.CreateDirectory(configDirectory);
				Directory.CreateDirectory(contentDirectory);
				if (ServiceFactoryBase.ContentService != null)
					ServiceFactoryBase.ContentService.Invalidate();

				if (ConnectionSettingsManager.IsRemote)
				{
					var configFileName = Path.Combine(configDirectory, "Config.fscp");
					var configFileStream = File.Create(configFileName);
					var stream = FiresecService.GetConfig();
					CopyStream(stream, configFileStream);
					LoadFromZipFile(configFileName);

					var result = FiresecService.GetSecurityConfiguration();
					if (!result.HasError && result.Result != null)
					{
						SecurityConfiguration = result.Result;
					}
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

					if (File.Exists(serverConfigDirectory + "\\..\\SecurityConfiguration.xml"))
					{
						File.Copy(serverConfigDirectory + "\\..\\SecurityConfiguration.xml", Path.Combine(configDirectory, "SecurityConfiguration.xml"), true);
					}

					LoadConfigFromDirectory(configDirectory);
					SecurityConfiguration = ZipSerializeHelper.DeSerialize<SecurityConfiguration>(Path.Combine(configDirectory, "SecurityConfiguration.xml"));
				}
				UpdateConfiguration();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.GetConfiguration");
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
				UpdatePlansConfiguration();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.UpdateConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		public static void UpdatePlansConfiguration()
		{
			try
			{
				GKManager.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.Zones.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.Directions.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.MPTs.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.DeviceConfiguration.GuardZones.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				GKManager.Doors.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });

				SystemConfiguration.Cameras.ForEach(x => x.PlanElementUIDs = new List<Guid>());
				SystemConfiguration.AutomationConfiguration.Procedures.ForEach(x => x.PlanElementUIDs = new List<Guid>());

				var gkDeviceMap = new Dictionary<Guid, GKDevice>();
				foreach (var device in GKManager.Devices)
				{
					if (!gkDeviceMap.ContainsKey(device.UID))
						gkDeviceMap.Add(device.UID, device);
				}
				var gkZoneMap = new Dictionary<Guid, GKZone>();
				foreach (var zone in GKManager.Zones)
				{
					if (!gkZoneMap.ContainsKey(zone.UID))
						gkZoneMap.Add(zone.UID, zone);
				}
				var gkGuardZoneMap = new Dictionary<Guid, GKGuardZone>();
				foreach (var guardZone in GKManager.DeviceConfiguration.GuardZones)
				{
					if (!gkGuardZoneMap.ContainsKey(guardZone.UID))
						gkGuardZoneMap.Add(guardZone.UID, guardZone);
				}
				var gkDirectionMap = new Dictionary<Guid, GKDirection>();
				foreach (var direction in GKManager.Directions)
				{
					if (!gkDirectionMap.ContainsKey(direction.UID))
						gkDirectionMap.Add(direction.UID, direction);
				}
				var gkDelayMap = new Dictionary<Guid, GKDelay>();
				foreach (var delay in GKManager.Delays)
				{
					if (!gkDelayMap.ContainsKey(delay.UID))
						gkDelayMap.Add(delay.UID, delay);
				}
				var gkPumpStationMap = new Dictionary<Guid, GKPumpStation>();
				foreach (var pumpStation in GKManager.PumpStations)
				{
					if (!gkPumpStationMap.ContainsKey(pumpStation.UID))
						gkPumpStationMap.Add(pumpStation.UID, pumpStation);
				}
				var gkMPTMap = new Dictionary<Guid, GKMPT>();
				foreach (var mpt in GKManager.MPTs)
				{
					if (!gkMPTMap.ContainsKey(mpt.UID))
						gkMPTMap.Add(mpt.UID, mpt);
				}
				var gkDoorMap = new Dictionary<Guid, GKDoor>();
				foreach (var door in GKManager.Doors)
				{
					if (!gkDoorMap.ContainsKey(door.UID))
						gkDoorMap.Add(door.UID, door);
				}

				var cameraMap = new Dictionary<Guid, Camera>();
				foreach (var camera in SystemConfiguration.Cameras)
				{
					if (!cameraMap.ContainsKey(camera.UID))
						cameraMap.Add(camera.UID, camera);
				}

				var procedureMap = new Dictionary<Guid, Procedure>();
				foreach (var procedure in ClientManager.SystemConfiguration.AutomationConfiguration.Procedures)
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
					foreach (var zone in plan.ElementPolygonGKZones)
					{
						UpdateZoneType(zone, zone.ZoneUID != Guid.Empty && gkZoneMap.ContainsKey(zone.ZoneUID) ? gkZoneMap[zone.ZoneUID] : null);
						if (gkZoneMap.ContainsKey(zone.ZoneUID))
							gkZoneMap[zone.ZoneUID].PlanElementUIDs.Add(zone.UID);
					}
					foreach (var zone in plan.ElementRectangleGKZones)
					{
						UpdateZoneType(zone, zone.ZoneUID != Guid.Empty && gkZoneMap.ContainsKey(zone.ZoneUID) ? gkZoneMap[zone.ZoneUID] : null);
						if (gkZoneMap.ContainsKey(zone.ZoneUID))
							gkZoneMap[zone.ZoneUID].PlanElementUIDs.Add(zone.UID);
					}
					foreach (var guardZone in plan.ElementPolygonGKGuardZones)
					{
						UpdateZoneType(guardZone, guardZone.ZoneUID != Guid.Empty && gkGuardZoneMap.ContainsKey(guardZone.ZoneUID) ? gkGuardZoneMap[guardZone.ZoneUID] : null);
						if (gkGuardZoneMap.ContainsKey(guardZone.ZoneUID))
							gkGuardZoneMap[guardZone.ZoneUID].PlanElementUIDs.Add(guardZone.UID);
					}
					foreach (var guardZone in plan.ElementRectangleGKGuardZones)
					{
						UpdateZoneType(guardZone, guardZone.ZoneUID != Guid.Empty && gkGuardZoneMap.ContainsKey(guardZone.ZoneUID) ? gkGuardZoneMap[guardZone.ZoneUID] : null);
						if (gkGuardZoneMap.ContainsKey(guardZone.ZoneUID))
							gkGuardZoneMap[guardZone.ZoneUID].PlanElementUIDs.Add(guardZone.UID);
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
					foreach (var delay in plan.ElementRectangleGKDelays)
					{
						UpdateDelayType(delay, delay.DelayUID != Guid.Empty && gkDelayMap.ContainsKey(delay.DelayUID) ? gkDelayMap[delay.DelayUID] : null);
						if (gkDelayMap.ContainsKey(delay.DelayUID))
							gkDelayMap[delay.DelayUID].PlanElementUIDs.Add(delay.UID);
					}
					foreach (var delay in plan.ElementPolygonGKDelays)
					{
						UpdateDelayType(delay, delay.DelayUID != Guid.Empty && gkDelayMap.ContainsKey(delay.DelayUID) ? gkDelayMap[delay.DelayUID] : null);
						if (gkDelayMap.ContainsKey(delay.DelayUID))
							gkDelayMap[delay.DelayUID].PlanElementUIDs.Add(delay.UID);
					}
					foreach (var pumpStation in new IElementPumpStation[0]
						.Concat(plan.ElementPolygonGKPumpStations)
						.Concat(plan.ElementRectangleGKPumpStations))
					{
						UpdatePumpStationType(pumpStation, pumpStation.PumpStationUID != Guid.Empty && gkPumpStationMap.ContainsKey(pumpStation.PumpStationUID) ? gkPumpStationMap[pumpStation.PumpStationUID] : null);
						if (gkPumpStationMap.ContainsKey(pumpStation.PumpStationUID))
							gkPumpStationMap[pumpStation.PumpStationUID].PlanElementUIDs.Add(pumpStation.UID);
					}
					foreach (var direction in plan.ElementRectangleGKDirections)
					{
						UpdateDirectionType(direction, direction.DirectionUID != Guid.Empty && gkDirectionMap.ContainsKey(direction.DirectionUID) ? gkDirectionMap[direction.DirectionUID] : null);
						if (gkDirectionMap.ContainsKey(direction.DirectionUID))
							gkDirectionMap[direction.DirectionUID].PlanElementUIDs.Add(direction.UID);
					}
					foreach (var direction in plan.ElementPolygonGKDirections)
					{
						UpdateDirectionType(direction, direction.DirectionUID != Guid.Empty && gkDirectionMap.ContainsKey(direction.DirectionUID) ? gkDirectionMap[direction.DirectionUID] : null);
						if (gkDirectionMap.ContainsKey(direction.DirectionUID))
							gkDirectionMap[direction.DirectionUID].PlanElementUIDs.Add(direction.UID);
					}
					foreach (var mpt in plan.ElementRectangleGKMPTs)
					{
						UpdateMPTType(mpt, mpt.MPTUID != Guid.Empty && gkMPTMap.ContainsKey(mpt.MPTUID) ? gkMPTMap[mpt.MPTUID] : null);
						if (gkMPTMap.ContainsKey(mpt.MPTUID))
							gkMPTMap[mpt.MPTUID].PlanElementUIDs.Add(mpt.UID);
					}
					foreach (var mpt in plan.ElementPolygonGKMPTs)
					{
						UpdateMPTType(mpt, mpt.MPTUID != Guid.Empty && gkMPTMap.ContainsKey(mpt.MPTUID) ? gkMPTMap[mpt.MPTUID] : null);
						if (gkMPTMap.ContainsKey(mpt.MPTUID))
							gkMPTMap[mpt.MPTUID].PlanElementUIDs.Add(mpt.UID);
					}
					for (int i = plan.ElementGKDoors.Count(); i > 0; i--)
					{
						var elementGKDoor = plan.ElementGKDoors[i - 1];
						elementGKDoor.UpdateZLayer();
						if (gkDoorMap.ContainsKey(elementGKDoor.DoorUID))
							gkDoorMap[elementGKDoor.DoorUID].PlanElementUIDs.Add(elementGKDoor.UID);
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
					foreach (var subplan in plan.ElementPolygonSubPlans)
						UpdateSubPlan(subplan, subplan.PlanUID != Guid.Empty && planMap.ContainsKey(subplan.PlanUID) ? planMap[subplan.PlanUID] : null);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.UpdatePlansConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		private static void UpdateZoneType(IElementZone elementZone, GKGuardZone zone)
		{
			elementZone.SetZLayer(zone == null ? 20 : 40);
			elementZone.BackgroundColor = zone == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Brown;
		}
		private static void UpdateZoneType(IElementZone elementZone, GKZone zone)
		{
			elementZone.SetZLayer(zone == null ? 50 : 60);
			elementZone.BackgroundColor = zone == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Green;
		}
		private static void UpdateSKDZoneType(IElementZone elementZone, GKSKDZone zone)
		{
			elementZone.SetZLayer(zone == null ? 50 : 60);
			elementZone.BackgroundColor = zone == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Green;
		}
		private static void UpdateDelayType(IElementDelay elementGKDelay, GKDelay gkDelay)
		{
			elementGKDelay.BackgroundColor = gkDelay == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.LightBlue;
		}
		private static void UpdatePumpStationType(IElementPumpStation elementGKPumpStation, GKPumpStation gkPumpStation)
		{
			elementGKPumpStation.BackgroundColor = (gkPumpStation == null) ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Cyan;
		}
		private static void UpdateDirectionType(IElementDirection elementGKDirection, GKDirection gkDirection)
		{
			elementGKDirection.SetZLayer(gkDirection == null ? 10 : 11);
			elementGKDirection.BackgroundColor = gkDirection == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.LightBlue;
		}
		private static void UpdateMPTType(IElementMPT elementGKMPT, GKMPT gkMPT)
		{
			elementGKMPT.SetZLayer(gkMPT == null ? 10 : 11);
			elementGKMPT.BackgroundColor = gkMPT == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.LightBlue;
		}
		private static void UpdateSubPlan(ElementRectangleSubPlan elementSubPlan, Plan plan)
		{
			elementSubPlan.BackgroundColor = plan == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Green;
		}
		private static void UpdateSubPlan(ElementPolygonSubPlan elementSubPlan, Plan plan)
		{
			elementSubPlan.BackgroundColor = (plan == null) ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Green;
		}

		public static void InvalidateContent()
		{
			var uids = new HashSet<Guid?>();
			foreach (var plan in PlansConfiguration.AllPlans)
			{
				uids.Add(plan.BackgroundImageSource);
				uids.Add(plan.BackgroundSVGImageSource);
				plan.AllElements.ForEach(x => uids.Add(x.BackgroundImageSource));
				plan.AllElements.ForEach(x => uids.Add(x.BackgroundSVGImageSource));
			}
			foreach (var layout in LayoutsConfiguration.Layouts)
			{
				foreach (var part in layout.Parts)
				{
					if (part.Properties is LayoutPartImageProperties)
					{
						var layoutPartImageProperties = part.Properties as LayoutPartImageProperties;
						uids.Add(layoutPartImageProperties.ReferenceUID);
						uids.Add(layoutPartImageProperties.ReferenceSVGUID);
					}
				}
			}
			SystemConfiguration.AutomationConfiguration.AutomationSounds.ForEach(x => uids.Add(x.Uid));
			uids.Remove(null);
			uids.Remove(Guid.Empty);
			ServiceFactoryBase.ContentService.RemoveAllBut(uids.Select(x => x.Value.ToString()).ToList());
		}
	}
}