using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;
using System;

namespace FiresecClient
{
	public partial class XManager
	{
		public static void GetConfiguration()
		{
			DeviceConfiguration = FiresecManager.FiresecService.GetXDeviceConfiguration();
			UpdateConfiguration();
		}

		public static void Invalidate()
		{
			ClearAllReferences();
			InitializeDevicesInZone();
			InitializeZoneLogic();
			InitializeDirections();
		}

		static void ClearAllReferences()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				device.Zones = new List<XZone>();
				device.Directions = new List<XDirection>();
			}
			foreach (var zone in DeviceConfiguration.Zones)
			{
				zone.Devices = new List<XDevice>();
				zone.Directions = new List<XDirection>();
			}
			foreach (var direction in DeviceConfiguration.Directions)
			{
				direction.Zones = new List<XZone>();
				direction.Devices = new List<XDevice>();
			}
		}

		static void InitializeDevicesInZone()
		{
			foreach (var device in DeviceConfiguration.Devices)
			{
				var zoneUIDs = new List<Guid>();
				if (device.ZoneUIDs != null)
					foreach (var zoneUID in device.ZoneUIDs)
					{
						var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
						if (zone != null)
						{
							zoneUIDs.Add(zoneUID);
							device.Zones.Add(zone);
							zone.Devices.Add(device);
						}
					}
				device.ZoneUIDs = zoneUIDs;
			}
		}

		static void InitializeZoneLogic()
		{
			foreach (var logicDevice in DeviceConfiguration.Devices)
			{
				var clauses = new List<XClause>();
				if (logicDevice.DeviceLogic.Clauses != null)
					foreach (var clause in logicDevice.DeviceLogic.Clauses)
					{
						clause.Devices = new List<XDevice>();
						clause.Zones = new List<XZone>();
						clause.Directions = new List<XDirection>();

						var zoneUIDs = new List<Guid>();
						foreach (var zoneUID in clause.ZoneUIDs)
						{
							var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
							if (zone != null)
							{
								zoneUIDs.Add(zoneUID);
								clause.Zones.Add(zone);
								//zone.DevicesInLogic.Add(logicDevice);
							}
						}
						clause.ZoneUIDs = zoneUIDs;

						var deviceUIDs = new List<Guid>();
						foreach (var deviceUID in clause.DeviceUIDs)
						{
							var device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
							if (device != null)
							{
								deviceUIDs.Add(deviceUID);
								clause.Devices.Add(device);
								//device.DevicesInLogic.Add(logicDevice);
							}
						}
						clause.DeviceUIDs = deviceUIDs;

						var directionUIDs = new List<Guid>();
						foreach (var directionUID in clause.DirectionUIDs)
						{
							var direction = DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == directionUID);
							if (direction != null)
							{
								directionUIDs.Add(directionUID);
								clause.Directions.Add(direction);
								//direction.DevicesInLogic.Add(logicDevice);
							}
						}
						clause.DirectionUIDs = directionUIDs;

						if (clause.Zones.Count > 0 || clause.Devices.Count > 0 || clause.Directions.Count > 0)
							clauses.Add(clause);
					}
				logicDevice.DeviceLogic.Clauses = clauses;
			}
		}

		static void InitializeDirections()
		{
			foreach (var direction in DeviceConfiguration.Directions)
			{
				var deviceUIDs = new List<Guid>();
				foreach (var deviceUID in direction.DeviceUIDs)
				{
					var device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
					if (device != null)
					{
						deviceUIDs.Add(deviceUID);
						direction.Devices.Add(device);
						device.Directions.Add(direction);
					}
				}
				direction.DeviceUIDs = deviceUIDs;

				var zoneUIDs = new List<Guid>();
				foreach (var zoneUID in direction.ZoneUIDs)
				{
					var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						zoneUIDs.Add(zoneUID);
						direction.Zones.Add(zone);
						zone.Directions.Add(direction);
					}
				}
				direction.ZoneUIDs = zoneUIDs;
			}
		}
	}
}