using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using System.Linq;

namespace GKProcessor
{
	public class KauDatabase : CommonDatabase
	{
		public List<GKDevice> Devices { get; set; }
		List<GKZone> Zones { get; set; }

		public KauDatabase(GKDevice kauDevice)
		{
			Devices = new List<GKDevice>();
			Zones = new List<GKZone>();
			DatabaseType = DatabaseType.Kau;
			RootDevice = kauDevice;

			AllDevices = new List<GKDevice>();
			AddChild(RootDevice);

			foreach (var device in AllDevices)
			{
				device.KauDatabaseParent = RootDevice;
				device.KAUDescriptorNo = NextDescriptorNo;
				Devices.Add(device);
			}

			foreach (var zone in GKManager.Zones)
			{
				if (zone.GkDatabaseParent == RootDevice.GKParent)
				{
					if (zone.Devices.Any(x => x.KauDatabaseParent != RootDevice))
						continue;
					zone.KAUDescriptorNo = NextDescriptorNo;
					zone.KauDatabaseParent = RootDevice;
					Zones.Add(zone);
				}
			}
		}

		List<GKDevice> AllDevices;
		void AddChild(GKDevice device)
		{
			if (device.IsNotUsed)
				return;

			if (device.IsRealDevice)
				AllDevices.Add(device);

			foreach (var child in device.Children)
			{
				AddChild(child);
			}
		}

		public override void BuildObjects()
		{
			Descriptors = new List<BaseDescriptor>();
			foreach (var device in Devices)
			{
				var deviceDescriptor = new DeviceDescriptor(device, DatabaseType);
				Descriptors.Add(deviceDescriptor);
			}
			foreach (var zone in Zones)
			{
				var zoneDescriptor = new ZoneDescriptor(zone);
				zoneDescriptor.DatabaseType = DatabaseType.Kau;
				zoneDescriptor.GKBase.KauDatabaseParent = RootDevice;
				Descriptors.Add(zoneDescriptor);
			}
			foreach (var descriptor in Descriptors)
			{
				descriptor.Build();
				descriptor.InitializeAllBytes();
			}
		}
	}
}