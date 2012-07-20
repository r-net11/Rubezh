using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace Commom.GK
{
	public class DatabaseCollection
	{
		public List<KauDatabase> KauDatabases { get; private set; }
		public List<GkDatabase> GkDatabases { get; private set; }

		public DatabaseCollection()
		{
			CreateDBs();
			CreateDevicesInKau();
			CreateDevicesInGkForZones();
			CreateDevicesInGkForLogic();
			CreateZones();

			foreach (var kauDatabase in KauDatabases)
			{
				kauDatabase.BuildObjects();
			}
			foreach (var gkDatabase in GkDatabases)
			{
				gkDatabase.BuildObjects();
			}
		}

		void CreateDBs()
		{
			KauDatabases = new List<KauDatabase>();
			GkDatabases = new List<GkDatabase>();

			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == XDriverType.KAU)
				{
					var kauDatabase = new KauDatabase(device);
					KauDatabases.Add(kauDatabase);
				}

				if (device.Driver.DriverType == XDriverType.GK)
				{
					var gkDatabase = new GkDatabase(device);
					GkDatabases.Add(gkDatabase);
				}
			}
		}

		void CreateDevicesInKau()
		{
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsDeviceOnShleif)
				{
					var kauParent = device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
					var kauDatabase = KauDatabases.FirstOrDefault(x => x.RootDevice.UID == kauParent.UID);
					kauDatabase.AddDevice(device);
				}
			}
		}

		void CreateDevicesInGkForZones()
		{
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (zone.GkDatabaseParent != null)
				{
					var commonDatabase = GetDatabase(zone.GkDatabaseParent);
					foreach (var device in zone.Devices)
					{
						commonDatabase.AddDevice(device);
					}
				}
			}
		}

		void CreateDevicesInGkForLogic()
		{
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				var kauParents = new HashSet<XDevice>();

				foreach (var dependentDevice in device.DeviceLogic.DependentDevices)
				{
					var kauParent = dependentDevice.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
					kauParents.Add(kauParent);
				}

				foreach (var dependentZone in device.DeviceLogic.DependentZones)
				{
					if (dependentZone.KauDatabaseParent != null)
					{
						kauParents.Add(dependentZone.KauDatabaseParent);
					}
					if (dependentZone.GkDatabaseParent != null)
					{
						kauParents.Add(dependentZone.GkDatabaseParent);
					}
				}

				if (kauParents.Count > 1)
				{
					var kauDevice = kauParents.ToList()[0];
					var commonDatabase = GetDatabase(kauDevice);
					commonDatabase.AddDevice(device);

					foreach (var dependentDevice in device.DeviceLogic.DependentDevices)
					{
						commonDatabase.AddDevice(dependentDevice);
					}
					foreach (var dependentZone in device.DeviceLogic.DependentZones)
					{
						commonDatabase.AddZone(dependentZone);
					}
				}
			}
		}

		void CreateZones()
		{
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (zone.KauDatabaseParent != null)
				{
					var kauDatabase = GetDatabase(zone.KauDatabaseParent);
					kauDatabase.AddZone(zone);
				}
				if (zone.GkDatabaseParent != null)
				{
					var gkDatabase = GetDatabase(zone.GkDatabaseParent);
					gkDatabase.AddZone(zone);
				}
			}
		}

		CommonDatabase GetDatabase(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.KAU)
			{
				var kauDatabase = KauDatabases.FirstOrDefault(x => x.RootDevice == device);
				return kauDatabase;
			}

			if (device.Driver.DriverType == XDriverType.GK)
			{
				var gkDatabase = GkDatabases.FirstOrDefault(x => x.RootDevice == device);
				return gkDatabase;
			}

			return null;
		}
	}
}