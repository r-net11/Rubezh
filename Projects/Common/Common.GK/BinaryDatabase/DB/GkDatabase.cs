using System.Collections.Generic;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public class GkDatabase : CommonDatabase
	{
		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }
		public List<XDirection> Directions { get; set; }
		public List<XDelay> Delays { get; set; }
		public List<KauDatabase> KauDatabases { get; set; }

		public GkDatabase(XDevice gkDevice)
		{
			Devices = new List<XDevice>();
			Zones = new List<XZone>();
			Directions = new List<XDirection>();
			Delays = new List<XDelay>();
			KauDatabases = new List<KauDatabase>();
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
			Devices.ForEach(x => x.GkDatabaseParent = RootDevice);
		}

		public void AddDevice(XDevice device)
		{
			if (!Devices.Contains(device))
			{
				device.GKDescriptorNo = NextDescriptorNo;
				Devices.Add(device);
			}
		}

		public void AddZone(XZone zone)
		{
			if (!Zones.Contains(zone))
			{
				zone.GKDescriptorNo = NextDescriptorNo;
				Zones.Add(zone);
			}
		}

		public void AddDirection(XDirection direction)
		{
			if (!Directions.Contains(direction))
			{
				direction.GKDescriptorNo = NextDescriptorNo;
				Directions.Add(direction);
			}
		}

		public void AddDelay(XDelay delay)
		{
			if (!Delays.Contains(delay))
			{
				delay.GKDescriptorNo = NextDescriptorNo;
				delay.GkDatabaseParent = RootDevice;
				Delays.Add(delay);
			}
		}

		public override void BuildObjects()
		{
			AddKauObjects();
			foreach (var zone in XManager.Zones)
			{
				if (zone.GkDatabaseParent == RootDevice)
				{
					AddZone(zone);
				}
			}
			foreach (var direction in XManager.Directions)
			{
				if (direction.GkDatabaseParent == RootDevice)
				{
					AddDirection(direction);
				}
			}

			Descriptors = new List<BaseDescriptor>();
			foreach (var device in Devices)
			{
				var deviceDescriptor = new DeviceDescriptor(device, DatabaseType);
				Descriptors.Add(deviceDescriptor);
			}
			foreach (var zone in Zones)
			{
				var zoneDescriptor = new ZoneDescriptor(zone);
				Descriptors.Add(zoneDescriptor);
			}
			foreach (var direction in Directions)
			{
				var directionDescriptor = new DirectionDescriptor(direction);
				Descriptors.Add(directionDescriptor);
			}

			foreach (var direction in Directions)
			{
				if (direction.IsNS)
				{
					var pumpStationCreator = new PumpStationCreator(this, direction);
					pumpStationCreator.Create();
				}
			}

			foreach (var descriptor in Descriptors)
			{
				descriptor.InitializeAllBytes();
			}
		}

		void AddKauObjects()
		{
			foreach (var kauDatabase in KauDatabases)
			{
				foreach (var descriptor in kauDatabase.Descriptors)
				{
					var xBase = descriptor.XBase;
					xBase.GkDatabaseParent = RootDevice;
					if (xBase is XDevice)
					{
						XDevice device = xBase as XDevice;
						AddDevice(device);
					}
				}
			}
		}
	}
}