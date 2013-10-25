using System.Collections.Generic;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public class GkDatabase : CommonDatabase
	{
		public List<KauDatabase> KauDatabases { get; set; }

		public GkDatabase(XDevice gkDevice)
		{
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

			BinaryObjects = new List<BinaryObjectBase>();
			foreach (var device in Devices)
			{
				var deviceBinaryObject = new DeviceBinaryObject(device, DatabaseType);
				BinaryObjects.Add(deviceBinaryObject);
			}
			foreach (var zone in Zones)
			{
				var zoneBinaryObject = new ZoneBinaryObject(zone);
				BinaryObjects.Add(zoneBinaryObject);
			}
			foreach (var direction in Directions)
			{
				var directionBinaryObject = new DirectionBinaryObject(direction, DatabaseType);
				BinaryObjects.Add(directionBinaryObject);
			}

			foreach (var direction in Directions)
			{
				if (direction.IsNS)
				{
					var pumpStationCreator = new PumpStationCreator(this, direction);
					pumpStationCreator.Create();
				}
			}

			foreach (var binaryObject in BinaryObjects)
			{
				binaryObject.InitializeAllBytes();
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