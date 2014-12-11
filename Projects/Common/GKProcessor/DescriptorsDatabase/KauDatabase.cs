using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using System.Linq;

namespace GKProcessor
{
	public class KauDatabase : CommonDatabase
	{
		public KauDatabase(GKDevice kauDevice)
		{
			DatabaseType = DatabaseType.Kau;
			RootDevice = kauDevice;

			AllDevices = new List<GKDevice>();
			AddChild(RootDevice);

			foreach (var device in AllDevices)
			{
				device.KAUDescriptorNo = NextDescriptorNo;
				device.KauDatabaseParent = RootDevice;
				Devices.Add(device);
			}

			foreach (var zone in GKManager.Zones.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				zone.KauDatabaseParent = RootDevice;
				zone.KAUDescriptorNo = NextDescriptorNo;
				Zones.Add(zone);
			}

			foreach (var guardZone in GKManager.GuardZones.FindAll(x => x.KauDatabaseParent == RootDevice && x.GuardZoneEnterMethod == GKGuardZoneEnterMethod.GlobalOnly))
			{
				guardZone.KauDatabaseParent = RootDevice;
				GuardZones.Add(guardZone);
			}

			foreach (var direction in GKManager.Directions.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				direction.KauDatabaseParent = RootDevice;
				direction.KAUDescriptorNo = NextDescriptorNo;
				Directions.Add(direction);
			}

			foreach (var delay in GKManager.Delays.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				delay.KauDatabaseParent = RootDevice;
				delay.KAUDescriptorNo = NextDescriptorNo;
				Delays.Add(delay);
			}

			foreach (var pumpStation in GKManager.PumpStations.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				pumpStation.KauDatabaseParent = RootDevice;
				PumpStations.Add(pumpStation);
			}

			foreach (var mpt in GKManager.DeviceConfiguration.MPTs.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				mpt.KauDatabaseParent = RootDevice;
				MPTs.Add(mpt);
			}

			foreach (var code in GKManager.DeviceConfiguration.Codes)
			{
				if (code.GkDatabaseParent == RootDevice)
				{					
					Codes.Add(code);
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
				var zoneDescriptor = new ZoneDescriptor(zone, DatabaseType.Kau);
				Descriptors.Add(zoneDescriptor);
			}
			foreach (var delay in Delays)
			{
				var delayDescriptor = new DelayDescriptor(delay, DatabaseType.Kau);
				Descriptors.Add(delayDescriptor);
			}
			foreach (var guardZone in GuardZones)
			{
				guardZone.KAUDescriptorNo = NextDescriptorNo;
				var guardZoneDescriptor = new GuardZoneDescriptor(guardZone, DatabaseType.Kau);
				Descriptors.Add(guardZoneDescriptor);

				if (guardZone.Pim != null)
				{
					AddPim(guardZone.Pim);
					Descriptors.Add(guardZoneDescriptor.GuardZonePimDescriptor);
				}
			}
			foreach (var direction in Directions)
			{
				var directionDescriptor = new DirectionDescriptor(direction, DatabaseType.Kau);
				Descriptors.Add(directionDescriptor);
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.KAUDescriptorNo = NextDescriptorNo;
				var pumpStationDescriptor = new PumpStationDescriptor(this, pumpStation, DatabaseType.Kau);
				Descriptors.Add(pumpStationDescriptor);

				var pumpStationCreator = new PumpStationCreator(this, pumpStation, DatabaseType.Kau);
				pumpStationCreator.Create();
			}

			foreach (var mpt in MPTs)
			{
				mpt.KAUDescriptorNo = NextDescriptorNo;
				var mptDescriptor = new MPTDescriptor(this, mpt, DatabaseType.Kau);
				Descriptors.Add(mptDescriptor);

				var mptCreator = new MPTCreator(this, mpt, DatabaseType.Kau);
				mptCreator.Create();
			}

			foreach (var code in Codes)
			{
				code.KAUDescriptorNo = NextDescriptorNo;
				var codeDescriptor = new CodeDescriptor(code);
				Descriptors.Add(codeDescriptor);
			}

			foreach (var descriptor in Descriptors)
			{
				descriptor.Build();
				descriptor.InitializeAllBytes();
			}
		}
	}
}