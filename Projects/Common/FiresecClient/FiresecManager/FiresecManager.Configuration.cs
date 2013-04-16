using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
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
				PlansConfiguration.Update();
				FiresecConfiguration.UpdateConfiguration();
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
				var deviceMap = new Dictionary<Guid, Device>();
				FiresecConfiguration.DeviceConfiguration.Devices.ForEach(device => deviceMap.Add(device.UID, device));
				var xdeviceMap = new Dictionary<Guid, XDevice>();
				XManager.DeviceConfiguration.Devices.ForEach(xdevice => xdeviceMap.Add(xdevice.UID, xdevice));
				var zoneMap = new Dictionary<Guid, Zone>();
				FiresecConfiguration.DeviceConfiguration.Zones.ForEach(zone => zoneMap.Add(zone.UID, zone));
				var xzoneMap = new Dictionary<Guid, XZone>();
				XManager.DeviceConfiguration.Zones.ForEach(xzone => xzoneMap.Add(xzone.UID, xzone));
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
						if (xdeviceMap.ContainsKey(elementXDevice.XDeviceUID))
							xdeviceMap[elementXDevice.XDeviceUID].PlanElementUIDs.Add(elementXDevice.UID);
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
						UpdateZoneType(xzone, xzone.ZoneUID != Guid.Empty && xzoneMap.ContainsKey(xzone.ZoneUID) ? xzoneMap[xzone.ZoneUID] : null);
					foreach (var xzone in plan.ElementRectangleXZones)
						UpdateZoneType(xzone, xzone.ZoneUID != Guid.Empty && xzoneMap.ContainsKey(xzone.ZoneUID) ? xzoneMap[xzone.ZoneUID] : null);
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

		public static List<XDriver> XDrivers
		{
			get
			{
				if (FiresecConfiguration == null || FiresecConfiguration.XDriversConfiguration == null || FiresecConfiguration.XDriversConfiguration.XDrivers == null)
				{
					Logger.Error("FiresecManager XDrivers = null");
					return new List<XDriver>();
				}
				return FiresecConfiguration.XDriversConfiguration.XDrivers;
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


		private static void UpdateZoneType(IElementZone elementZone, Zone zone)
		{
			elementZone.BackgroundColor = System.Windows.Media.Colors.Black;
			elementZone.SetZLayer(2);
			if (zone != null)
				switch (zone.ZoneType)
				{
					case ZoneType.Fire:
						elementZone.BackgroundColor = System.Windows.Media.Colors.Green;
						elementZone.SetZLayer(3);
						break;
					case ZoneType.Guard:
						elementZone.BackgroundColor = System.Windows.Media.Colors.Brown;
						elementZone.SetZLayer(4);
						break;
				}
		}
		private static void UpdateZoneType(IElementZone elementZone, XZone zone)
		{
			elementZone.SetZLayer(zone == null ? 5 : 6);
			elementZone.BackgroundColor = zone == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Green;
		}
		private static void UpdateSubPlan(ElementSubPlan elementSubPlan, Plan plan)
		{
			elementSubPlan.BackgroundColor = plan == null ? System.Windows.Media.Colors.Black : System.Windows.Media.Colors.Green;
		}
	}
}