using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrustructure.Plans.Elements;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static FiresecConfiguration FiresecConfiguration { get; set; }
		public static PlansConfiguration PlansConfiguration
		{
			get { return FiresecAPI.Models.ConfigurationCash.PlansConfiguration; }
			set { FiresecAPI.Models.ConfigurationCash.PlansConfiguration = value; }
		}

		public static DeviceLibraryConfiguration DeviceLibraryConfiguration { get; set; }
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
				var stream = FiresecManager.FiresecService.GetConfig();
				FiresecConfiguration = new FiresecConfiguration();

				var folderName = AppDataFolderHelper.GetLocalFolder(configurationFolderName);
				var configFileName = Path.Combine(folderName, "Config.fscp");
				if (Directory.Exists(folderName))
					Directory.Delete(folderName, true);
				Directory.CreateDirectory(folderName);
				if (ServiceFactoryBase.ContentService != null)
					ServiceFactoryBase.ContentService.Invalidate();

				var configFileStream = File.Create(configFileName);
				CopyStream(stream, configFileStream);
				bool isFullConfiguration;
				LoadFromZipFile(configFileName, out isFullConfiguration);

				UpdateConfiguration();
				FiresecConfiguration.CreateStates();
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
				FiresecConfiguration.UpdateConfiguration();
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
				FiresecConfiguration.DeviceConfiguration.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				FiresecConfiguration.DeviceConfiguration.Zones.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				XManager.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				XManager.Zones.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				XManager.Directions.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });

				SKDManager.Devices.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });
				SKDManager.Zones.ForEach(x => { x.PlanElementUIDs = new List<Guid>(); });

				FiresecManager.SystemConfiguration.Cameras.ForEach(x => x.PlanElementUIDs = new List<Guid>());

				var deviceMap = new Dictionary<Guid, Device>();
				FiresecConfiguration.DeviceConfiguration.Devices.ForEach(device => deviceMap.Add(device.UID, device));
				var zoneMap = new Dictionary<Guid, Zone>();
				FiresecConfiguration.DeviceConfiguration.Zones.ForEach(zone => zoneMap.Add(zone.UID, zone));

				var xDeviceMap = new Dictionary<Guid, XDevice>();
				foreach (var xDevice in XManager.Devices)
				{
					if (!xDeviceMap.ContainsKey(xDevice.BaseUID))
						xDeviceMap.Add(xDevice.BaseUID, xDevice);
				}
				var xZoneMap = new Dictionary<Guid, XZone>();
				foreach (var xzone in XManager.Zones)
				{
					if (!xZoneMap.ContainsKey(xzone.BaseUID))
					xZoneMap.Add(xzone.BaseUID, xzone);
				}
				var xDirectionMap = new Dictionary<Guid, XDirection>();
				foreach (var xdirection in XManager.Directions)
				{
					if (!xDirectionMap.ContainsKey(xdirection.BaseUID))
						xDirectionMap.Add(xdirection.BaseUID, xdirection);
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
				foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
				{
					if (!cameraMap.ContainsKey(camera.UID))
						cameraMap.Add(camera.UID, camera);
				}

				var planMap = new Dictionary<Guid, Plan>();
				FiresecManager.PlansConfiguration.AllPlans.ForEach(plan => planMap.Add(plan.UID, plan));
				foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
				{
					for (int i = plan.ElementDevices.Count(); i > 0; i--)
					{
						var elementDevice = plan.ElementDevices[i - 1];
						elementDevice.UpdateZLayer();
						if (deviceMap.ContainsKey(elementDevice.DeviceUID))
							deviceMap[elementDevice.DeviceUID].PlanElementUIDs.Add(elementDevice.UID);
					}
					for (int i = plan.ElementXDevices.Count(); i > 0; i--)
					{
						var elementXDevice = plan.ElementXDevices[i - 1];
						elementXDevice.UpdateZLayer();
						if (xDeviceMap.ContainsKey(elementXDevice.XDeviceUID))
							xDeviceMap[elementXDevice.XDeviceUID].PlanElementUIDs.Add(elementXDevice.UID);
					}

					foreach (var elementZone in plan.ElementPolygonZones)
					{
						UpdateZoneType(elementZone, elementZone.ZoneUID != Guid.Empty && zoneMap.ContainsKey(elementZone.ZoneUID) ? zoneMap[elementZone.ZoneUID] : null);
						if (zoneMap.ContainsKey(elementZone.ZoneUID))
							zoneMap[elementZone.ZoneUID].PlanElementUIDs.Add(elementZone.UID);
					}
					foreach (var elementZone in plan.ElementRectangleZones)
					{
						UpdateZoneType(elementZone, elementZone.ZoneUID != Guid.Empty && zoneMap.ContainsKey(elementZone.ZoneUID) ? zoneMap[elementZone.ZoneUID] : null);
						if (zoneMap.ContainsKey(elementZone.ZoneUID))
							zoneMap[elementZone.ZoneUID].PlanElementUIDs.Add(elementZone.UID);
					}
					foreach (var xzone in plan.ElementPolygonXZones)
					{
						UpdateZoneType(xzone, xzone.ZoneUID != Guid.Empty && xZoneMap.ContainsKey(xzone.ZoneUID) ? xZoneMap[xzone.ZoneUID] : null);
						if (xZoneMap.ContainsKey(xzone.ZoneUID))
							xZoneMap[xzone.ZoneUID].PlanElementUIDs.Add(xzone.UID);
					}
					foreach (var xzone in plan.ElementRectangleXZones)
					{
						UpdateZoneType(xzone, xzone.ZoneUID != Guid.Empty && xZoneMap.ContainsKey(xzone.ZoneUID) ? xZoneMap[xzone.ZoneUID] : null);
						if (xZoneMap.ContainsKey(xzone.ZoneUID))
							xZoneMap[xzone.ZoneUID].PlanElementUIDs.Add(xzone.UID);
					}
					foreach (var xdirection in plan.ElementRectangleXDirections)
					{
						UpdateDirectionType(xdirection, xdirection.DirectionUID != Guid.Empty && xDirectionMap.ContainsKey(xdirection.DirectionUID) ? xDirectionMap[xdirection.DirectionUID] : null);
						if (xDirectionMap.ContainsKey(xdirection.DirectionUID))
							xDirectionMap[xdirection.DirectionUID].PlanElementUIDs.Add(xdirection.UID);
					}
					foreach (var xdirection in plan.ElementPolygonXDirections)
					{
						UpdateDirectionType(xdirection, xdirection.DirectionUID != Guid.Empty && xDirectionMap.ContainsKey(xdirection.DirectionUID) ? xDirectionMap[xdirection.DirectionUID] : null);
						if (xDirectionMap.ContainsKey(xdirection.DirectionUID))
							xDirectionMap[xdirection.DirectionUID].PlanElementUIDs.Add(xdirection.UID);
					}

					for (int i = plan.ElementSKDDevices.Count(); i > 0; i--)
					{
						var elementSKDDevice = plan.ElementSKDDevices[i - 1];
						elementSKDDevice.UpdateZLayer();
						if (skdDeviceMap.ContainsKey(elementSKDDevice.DeviceUID))
							skdDeviceMap[elementSKDDevice.DeviceUID].PlanElementUIDs.Add(elementSKDDevice.UID);
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

		public static void SetEmptyConfiguration()
		{
			FiresecConfiguration.SetEmptyConfiguration();
		}

		public static List<Driver> Drivers
		{
			get
			{
				if (FiresecConfiguration == null || FiresecConfiguration.DriversConfiguration == null || FiresecConfiguration.DriversConfiguration.Drivers == null)
				{
					Logger.Error("FiresecManager Drivers = null");
					return new List<Driver>();
				}
				return FiresecConfiguration.DriversConfiguration.Drivers;
			}
		}

		public static List<Device> Devices
		{
			get
			{
				if (FiresecConfiguration == null || FiresecConfiguration.DeviceConfiguration == null || FiresecConfiguration.DeviceConfiguration.Devices == null)
				{
					Logger.Error("FiresecManager Devices = null");
					return new List<Device>();
				}
				return FiresecConfiguration.DeviceConfiguration.Devices;
			}
		}

		public static List<Zone> Zones
		{
			get
			{
				if (FiresecConfiguration == null || FiresecConfiguration.DeviceConfiguration == null || FiresecConfiguration.DeviceConfiguration.Zones == null)
				{
					Logger.Error("FiresecManager Zones = null");
					return new List<Zone>();
				}
				return FiresecConfiguration.DeviceConfiguration.Zones;
			}
		}

		public static List<Direction> Directions
		{
			get
			{
				if (FiresecConfiguration == null || FiresecConfiguration.DeviceConfiguration == null || FiresecConfiguration.DeviceConfiguration.Directions == null)
				{
					Logger.Error("FiresecManager Direction = null");
					return new List<Direction>();
				}
				return FiresecConfiguration.DeviceConfiguration.Directions;
			}
		}

		public static List<GuardUser> GuardUsers
		{
			get
			{
				if (FiresecConfiguration == null || FiresecConfiguration.DeviceConfiguration == null || FiresecConfiguration.DeviceConfiguration.GuardUsers == null)
				{
					Logger.Error("FiresecManager GuardUser = null");
					return new List<GuardUser>();
				}
				return FiresecConfiguration.DeviceConfiguration.GuardUsers;
			}
		}

		public static List<ParameterTemplate> ParameterTemplates
		{
			get
			{
				if (FiresecConfiguration.DeviceConfiguration.ParameterTemplates == null)
				{
					FiresecConfiguration.DeviceConfiguration.ParameterTemplates = new List<ParameterTemplate>();
				}
				return FiresecConfiguration.DeviceConfiguration.ParameterTemplates;
			}
		}

		private static void UpdateZoneType(IElementZone elementZone, Zone zone)
		{
			elementZone.BackgroundColor = System.Windows.Media.Colors.Black;
			elementZone.SetZLayer(20);
			if (zone != null)
				switch (zone.ZoneType)
				{
					case ZoneType.Fire:
						elementZone.BackgroundColor = System.Windows.Media.Colors.Green;
						elementZone.SetZLayer(30);
						break;
					case ZoneType.Guard:
						elementZone.BackgroundColor = System.Windows.Media.Colors.Brown;
						elementZone.SetZLayer(40);
						break;
				}
		}
		private static void UpdateZoneType(IElementZone elementZone, XZone zone)
		{
			elementZone.SetZLayer(zone == null ? 50 : 60);
			elementZone.BackgroundColor = zone == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Green;
		}
		private static void UpdateDirectionType(IElementDirection elementXDirection, XDirection xdirection)
		{
			elementXDirection.SetZLayer(xdirection == null ? 10 : 11);
			elementXDirection.BackgroundColor = xdirection == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.LightBlue;
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