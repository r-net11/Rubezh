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
				Devices.Add(device);
			}

			foreach (var delay in GKManager.Delays.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				delay.KauDatabaseParent = RootDevice;
				Delays.Add(delay);
			}

			foreach (var direction in GKManager.Directions.FindAll(x => x.KauDatabaseParent == RootDevice))
			{
				direction.KauDatabaseParent = RootDevice;
				Directions.Add(direction);
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
				device.KAUDescriptorNo = NextDescriptorNo;
				var deviceDescriptor = new DeviceDescriptor(device, DatabaseType);
				Descriptors.Add(deviceDescriptor);
			}

			foreach (var direction in Directions)
			{
				direction.KAUDescriptorNo = NextDescriptorNo;
				var directionDescriptor = new DirectionDescriptor(direction, DatabaseType.Kau);
				Descriptors.Add(directionDescriptor);
			}

			foreach (var delay in Delays)
			{
				delay.KAUDescriptorNo = NextDescriptorNo;
				var delayDescriptor = new DelayDescriptor(delay, DatabaseType.Kau);
				Descriptors.Add(delayDescriptor);
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

			foreach (var descriptor in Descriptors)
			{
				descriptor.Build();
				descriptor.InitializeAllBytes();
			}
		}
	}
}