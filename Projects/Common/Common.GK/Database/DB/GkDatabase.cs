using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecClient;
using Common.GK;

namespace Common.GK
{
	public class GkDatabase : CommonDatabase
	{
		public List<KauDatabase> KauDatabases { get; set; }
		public List<XDirection> Directions { get; set; }
		public List<XDevice> KauDevices { get; set; }
		public List<XZone> KauZones { get; set; }

		public GkDatabase(XDevice gkDevice)
		{
			KauDatabases = new List<KauDatabase>();
			Directions = new List<XDirection>();
			KauDevices = new List<XDevice>();
			KauZones = new List<XZone>();
			DatabaseType = DatabaseType.Gk;
			RootDevice = gkDevice;

			AddDevice(gkDevice);
			foreach (var device in gkDevice.Children)
			{
				if (device.Driver.DriverType == XDriverType.GKIndicator)
				{
					AddDevice(device);
				}
			}
			foreach (var device in gkDevice.Children)
			{
				if (device.Driver.DriverType == XDriverType.GKLine)
				{
					AddDevice(device);
				}
			}
			foreach (var device in gkDevice.Children)
			{
				if (device.Driver.DriverType == XDriverType.GKRele)
				{
					AddDevice(device);
				}
			}
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if ((device.Parent == null) || (device.Parent.Driver.DriverType == XDriverType.GK))
					continue;

				if (device.GkDatabaseParent == gkDevice)
				{
					AddDevice(device);
				}
			}
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (zone.GkDatabaseParent == gkDevice)
				{
					AddZone(zone);
				}
			}
		}

		public override void BuildObjects()
		{
			AddKauObjects();
			BinaryObjects = new List<BinaryObjectBase>();

			foreach (var device in Devices)
			{
				var deviceBinaryObject = new DeviceBinaryObject(device, DatabaseType);
				BinaryObjects.Add(deviceBinaryObject);
			}
			foreach (var zone in Zones)
			{
				var zoneBinaryObject = new ZoneBinaryObject(zone, DatabaseType);
				BinaryObjects.Add(zoneBinaryObject);
			}

			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
				if (direction.GkDatabaseParent == RootDevice)
				{
					direction.SetDatabaseNo(DatabaseType, NextChildNo);
					Directions.Add(direction);

					var directionBinaryObject = new DirectionBinaryObject(direction, DatabaseType);
					BinaryObjects.Add(directionBinaryObject);
				}
			}
		}

		void AddKauObjects()
		{
			foreach (var kauDatabase in KauDatabases)
			{
				foreach (var binaryObject in kauDatabase.BinaryObjects)
				{
					var binaryBase = binaryObject.BinaryBase;
					binaryBase.GkDatabaseParent = RootDevice;
					if (binaryBase is XDevice)
					{
						XDevice device = binaryBase as XDevice;
						AddDevice(device);
					}
					if (binaryBase is XZone)
					{
						XZone zone = binaryBase as XZone;
						AddZone(zone);
					}
				}
			}
		}
	}
}