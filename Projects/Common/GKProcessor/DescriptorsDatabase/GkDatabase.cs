using System.Collections.Generic;
using FiresecClient;
using XFiresecAPI;

namespace GKProcessor
{
	public class GkDatabase : CommonDatabase
	{
		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }
		public List<XDirection> Directions { get; set; }
		public List<XPumpStation> PumpStations { get; set; }
		public List<XDelay> Delays { get; set; }
		public List<XPim> Pims { get; set; }
		public List<KauDatabase> KauDatabases { get; set; }

		public GkDatabase(XDevice gkDevice)
		{
			Devices = new List<XDevice>();
			Zones = new List<XZone>();
			Directions = new List<XDirection>();
			PumpStations = new List<XPumpStation>();
			Delays = new List<XDelay>();
			Pims = new List<XPim>();
			KauDatabases = new List<KauDatabase>();
			DatabaseType = DatabaseType.Gk;
			RootDevice = gkDevice;

			AddDevice(gkDevice);
			foreach (var device in gkDevice.Children)
			{
				if (device.DriverType == XDriverType.GKIndicator)
				{
					AddDevice(device);
				}
			}
			foreach (var device in gkDevice.Children)
			{
				if (device.DriverType == XDriverType.GKLine)
				{
					AddDevice(device);
				}
			}
			foreach (var device in gkDevice.Children)
			{
				if (device.DriverType == XDriverType.GKRele)
				{
					AddDevice(device);
				}
			}
			Devices.ForEach(x => x.GkDatabaseParent = RootDevice);
		}

		void AddDevice(XDevice device)
		{
			if (!Devices.Contains(device))
			{
				device.GKDescriptorNo = NextDescriptorNo;
				Devices.Add(device);
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

		public void AddPim(XPim pim)
		{
			if (!Pims.Contains(pim))
			{
				pim.GKDescriptorNo = NextDescriptorNo;
				pim.GkDatabaseParent = RootDevice;
				Pims.Add(pim);
			}
		}

		public override void BuildObjects()
		{
			AddKauObjects();
			foreach (var zone in XManager.Zones)
			{
				if (zone.GkDatabaseParent == RootDevice)
				{
					zone.GKDescriptorNo = NextDescriptorNo;
					Zones.Add(zone);
				}
			}
			foreach (var direction in XManager.Directions)
			{
				if (direction.GkDatabaseParent == RootDevice)
				{
					direction.GKDescriptorNo = NextDescriptorNo;
					Directions.Add(direction);
				}
			}
			foreach (var pumpStation in XManager.PumpStations)
			{
				if (pumpStation.GkDatabaseParent == RootDevice)
				{
					//pumpStation.GKDescriptorNo = NextDescriptorNo;
					PumpStations.Add(pumpStation);
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
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.GKDescriptorNo = NextDescriptorNo;
				var pumpStationDescriptor = new PumpStationDescriptor(this, pumpStation);
				Descriptors.Add(pumpStationDescriptor);

				var pumpStationCreator = new PumpStationCreator(this, pumpStation, pumpStationDescriptor.MainDelay);
				pumpStationCreator.Create();
			}

			Descriptors.ForEach(x => x.InitializeAllBytes());
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