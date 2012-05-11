using System.Collections.Generic;
using XFiresecAPI;

namespace GKModule.Database
{
	public class CommonDatabase
	{
		short currentChildNo = 1;
		protected short NextChildNo
		{
			get { return currentChildNo++; }
		}

		public DatabaseType DatabaseType { get; protected set; }
		public XDevice RootDevice { get; protected set; }
		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }
		public List<BinaryObjectBase> BinaryObjects { get; set; }

		public CommonDatabase()
		{
			Devices = new List<XDevice>();
			Zones = new List<XZone>();
			BinaryObjects = new List<BinaryObjectBase>();
		}

		public virtual void AddDevice(XDevice device)
		{
			if (Devices.Contains(device))
				return;

			device.SetDatabaseNo(DatabaseType, NextChildNo);

			Devices.Add(device);
		}

		public virtual void AddZone(XZone zone)
		{
			if (Zones.Contains(zone))
				return;

			zone.SetDatabaseNo(DatabaseType, NextChildNo);

			Zones.Add(zone);
		}

		public void BuildObjects()
		{
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
		}
	}
}