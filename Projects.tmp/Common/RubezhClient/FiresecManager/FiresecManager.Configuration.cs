using Common;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.UpdatePlansConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		public static void InvalidateContent()
		{

		}
	}
}