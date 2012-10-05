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
                foreach(var zoneUID in device.ZoneUIDs)
                {
                    var zone = DeviceConfiguration.Zones.FirstOrDefault(x=>x.UID == zoneUID);
                    if (zone != null)
                    {
                        zoneUIDs.Add(zoneUID);
                        device.Zones.Add(zone);
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
                foreach (var clause in logicDevice.DeviceLogic.Clauses)
                {
                    clause.Devices = new List<XDevice>();
                    clause.Zones = new List<XZone>();
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

                    if (clause.Devices.Count > 0 || clause.Zones.Count > 0)
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

        //static void InitializeDirectionZones()
        //{
        //    foreach (var direction in DeviceConfiguration.Directions)
        //    {
        //        direction.Zones = new List<XZone>();
        //        for (int i = direction.ZoneUIDs.Count - 1; i >= 0; i--)
        //        {
        //            var zoneUID = direction.ZoneUIDs[i];
        //            var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
        //            if (zone != null)
        //                direction.Zones.Add(zone);
        //            else
        //                direction.ZoneUIDs.Remove(zoneUID);
        //        }
        //    }
        //}

        //static void InitializeDirectionDevices()
        //{
        //    foreach (var direction in DeviceConfiguration.Directions)
        //    {
        //        for (int i = direction.DeviceUIDs.Count - 1; i >= 0; i--)
        //        {
        //            var deviceUID = direction.DeviceUIDs[i];
        //            var device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
        //            if (device == null)
        //                direction.DeviceUIDs.Remove(deviceUID);
        //        }
        //    }
        //}

        //static void UpdateDeviceZones()
        //{
        //    foreach (var zone in DeviceConfiguration.Zones)
        //    {
        //        zone.Devices = new List<XDevice>();
        //    }

        //    foreach (var device in DeviceConfiguration.Devices)
        //    {
        //        device.Zones = new List<XZone>();
        //        foreach (var zoneUID in device.ZoneUIDs)
        //        {
        //            var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
        //            if (zone != null)
        //            {
        //                device.Zones.Add(zone);
        //                zone.Devices.Add(device);
        //            }
        //        }
        //    }
        //}

        //static void UpdateDirections()
        //{
        //    foreach (var zone in DeviceConfiguration.Zones)
        //    {
        //        zone.Directions = new List<XDirection>();
        //    }

        //    foreach (var direction in DeviceConfiguration.Directions)
        //    {
        //        direction.Zones = new List<XZone>();
        //        foreach (var zoneUID in direction.ZoneUIDs)
        //        {
        //            var zone = DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
        //            if (zone != null)
        //            {
        //                direction.Zones.Add(zone);
        //                zone.Directions.Add(direction);
        //            }
        //        }

        //        direction.Devices = new List<XDevice>();
        //        foreach (var deviceUID in direction.DeviceUIDs)
        //        {
        //            var device = DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
        //            if (device != null)
        //            {
        //                direction.Devices.Add(device);
        //            }
        //        }
        //    }
        //}
	}
}