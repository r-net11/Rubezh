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
				device.KauDatabaseParent = RootDevice;
				device.KAUDescriptorNo = NextDescriptorNo;
				Devices.Add(device);
			}

			foreach (var zone in GKManager.Zones.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				zone.KAUDescriptorNo = NextDescriptorNo;
				Zones.Add(zone);
			}

			foreach (var direction in GKManager.Directions.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				direction.KAUDescriptorNo = NextDescriptorNo;
				Directions.Add(direction);
			}

			foreach (var delay in GKManager.Delays.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				delay.KAUDescriptorNo = NextDescriptorNo;
				Delays.Add(delay);
			}

			foreach (var pumpStation in GKManager.PumpStations.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				pumpStation.KAUDescriptorNo = NextDescriptorNo;
				PumpStations.Add(pumpStation);
			}

			foreach (var mpt in GKManager.DeviceConfiguration.MPTs.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				mpt.KAUDescriptorNo = NextDescriptorNo;
				MPTs.Add(mpt);
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
				zoneDescriptor.GKBase.KauDatabaseParent = RootDevice;
				Descriptors.Add(zoneDescriptor);
			}
			foreach (var direction in Directions)
			{
				var directionDescriptor = new DirectionDescriptor(direction, DatabaseType.Kau);
				directionDescriptor.GKBase.KauDatabaseParent = RootDevice;
				Descriptors.Add(directionDescriptor);
			}
			foreach (var delay in Delays)
			{
				var delayDescriptor = new DelayDescriptor(delay, DatabaseType.Kau);
				delayDescriptor.GKBase.KauDatabaseParent = RootDevice;
				Descriptors.Add(delayDescriptor);
			}

			foreach (var pumpStation in PumpStations)
			{
				var pumpStationDescriptor = new PumpStationDescriptor(this, pumpStation, DatabaseType.Kau);
				Descriptors.Add(pumpStationDescriptor);

				var pumpStationCreator = new PumpStationCreator(this, pumpStation, pumpStationDescriptor.MainDelay, DatabaseType.Kau);
				pumpStationCreator.Create();
			}

			foreach (var mpt in MPTs)
			{
				var mptDescriptor = new MPTDescriptor(this, mpt);
				Descriptors.Add(mptDescriptor);

				var mptCreator = new MPTCreator(this, mpt, mptDescriptor.HandAutomaticOffPim, mptDescriptor.DoorAutomaticOffPim, mptDescriptor.FailureAutomaticOffPim);
				mptCreator.Create();
			}

			foreach (var descriptor in Descriptors)
			{
				descriptor.Build();
				descriptor.InitializeAllBytes();
			}
		}
	}
}