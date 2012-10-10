using System.Collections.Generic;
using XFiresecAPI;

namespace Common.GK
{
	public abstract class CommonDatabase
	{
		ushort currentChildNo = 1;
		protected ushort NextChildNo
		{
			get { return currentChildNo++; }
		}

		public DatabaseType DatabaseType { get; protected set; }
		public XDevice RootDevice { get; protected set; }
		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }
		public List<XDirection> Directions { get; set; }
		public List<BinaryObjectBase> BinaryObjects { get; set; }

		public CommonDatabase()
		{
			Devices = new List<XDevice>();
			Zones = new List<XZone>();
			Directions = new List<XDirection>();
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

		public virtual void AddDirection(XDirection direction)
		{
			if (Directions.Contains(direction))
				return;

			direction.SetDatabaseNo(DatabaseType, NextChildNo);
			Directions.Add(direction);
		}

		public abstract void BuildObjects();
	}
}