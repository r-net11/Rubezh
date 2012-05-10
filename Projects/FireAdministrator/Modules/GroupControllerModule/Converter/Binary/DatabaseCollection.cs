using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;

namespace GKModule.Converter.Binary
{
	public class DatabaseCollection
	{
		public List<KauDatabase> KauDatabases { get; set; }
		public List<GkDatabase> GkDatabases { get; set; }

		public void Build()
		{
			CreateDBs();
			CreateDevicesInKau();
			InitializeZones();
			CreateDevicesInGkForZones();
			InitializeDeviceLogicDependences();
			CreateDevicesInGkForLogic();
			CreateZones();
		}

		void CreateDBs()
		{
			KauDatabases = new List<KauDatabase>();
			GkDatabases = new List<GkDatabase>();

			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == XDriverType.KAU)
				{
					var kauDB = new KauDatabase(device);
					KauDatabases.Add(kauDB);
				}

				if (device.Driver.DriverType == XDriverType.GK)
				{
					var gkDBs = new GkDatabase(device);
					GkDatabases.Add(gkDBs);
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
					var kauDB = KauDatabases.FirstOrDefault(x => x.RootDevice.UID == kauParent.UID);
					if (kauParent.UID == kauDB.RootDevice.UID)
						kauDB.AddDevice(device);
				}
			}
		}

		void InitializeZones()
		{
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				zone.Devices = new List<XDevice>();
				zone.DBDevice = null;

				HashSet<XDevice> kauParents = new HashSet<XDevice>();
				foreach (var deviceUID in zone.DeviceUIDs)
				{
					var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
					zone.Devices.Add(device);

					var kauParent = device.AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.KAU);
					kauParents.Add(kauParent);
				}

				if (kauParents.Count == 1)
				{
					var kauDevice = kauParents.ToList()[0];
					zone.DBDevice = kauDevice;
				}
				if (kauParents.Count > 1)
				{
					var kauDevice = kauParents.ToList()[0];
					zone.DBDevice = kauDevice.Parent;
				}
			}
		}

		void CreateDevicesInGkForZones()
		{
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if ((zone.DBDevice != null) && (zone.DBDevice.Driver.DriverType == XDriverType.GK))
				{
					var baseDB = GetDatabase(zone.DBDevice);
					foreach (var device in zone.Devices)
					{
						baseDB.AddDevice(device);
					}
				}
			}
		}

		void InitializeDeviceLogicDependences()
		{
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				device.DeviceLogic.DependentDevices = new List<XDevice>();
				device.DeviceLogic.DependentZones = new List<XZone>();
			}
		}

		void CreateDevicesInGkForLogic()
		{
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				foreach (var dependentDevice in device.DeviceLogic.DependentDevices)
				{
				}

				foreach (var dependentZone in device.DeviceLogic.DependentZones)
				{
				}
			}
		}

		void CreateZones()
		{
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (zone.DBDevice != null)
				{
					var commonDatabase = GetDatabase(zone.DBDevice);
					commonDatabase.AddZone(zone);
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