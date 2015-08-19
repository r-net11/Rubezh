using FiresecAPI.GK;
using FiresecClient;
using System.Collections.Generic;
using System.Linq;

namespace GKProcessor
{
	public class KauDatabase : CommonDatabase
	{
		public KauDatabase(GKDevice kauDevice)
		{
			DatabaseType = DatabaseType.Kau;
			RootDevice = kauDevice;

			AddChild(RootDevice);
			Devices.ForEach(x => x.KauDatabaseParent = RootDevice);
		}

		void AddChild(GKDevice device)
		{
			if (device.IsNotUsed)
				return;

			if (device.IsRealDevice)
				Devices.Add(device);

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
				var deviceDescriptor = new DeviceDescriptor(device, DatabaseType.Kau);
				Descriptors.Add(deviceDescriptor);
			}

			foreach (var zone in GKManager.Zones.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var zoneDescriptor = new ZoneDescriptor(zone, DatabaseType.Kau);
				Descriptors.Add(zoneDescriptor);
			}

			foreach (var guardZone in GKManager.GuardZones.Where(x => x.KauDatabaseParent == RootDevice && !x.HasAccessLevel))
			{
				var guardZoneDescriptor = new GuardZoneDescriptor(guardZone, DatabaseType.Kau);
				Descriptors.Add(guardZoneDescriptor);

				if (guardZoneDescriptor.GuardZonePimDescriptor != null)
				{
					Descriptors.Add(guardZoneDescriptor.GuardZonePimDescriptor);
				}
			}

			foreach (var direction in GKManager.Directions.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var directionDescriptor = new DirectionDescriptor(direction, DatabaseType.Kau);
				Descriptors.Add(directionDescriptor);
			}

			foreach (var delay in GKManager.Delays.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var delayDescriptor = new DelayDescriptor(delay, DatabaseType.Kau);
				Descriptors.Add(delayDescriptor);
			}

			foreach (var pumpStation in GKManager.PumpStations.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var pumpStationDescriptor = new PumpStationDescriptor(this, pumpStation, DatabaseType.Kau);
				Descriptors.Add(pumpStationDescriptor);

				var pumpStationCreator = new PumpStationCreator(this, pumpStation, DatabaseType.Kau);
				pumpStationCreator.Create();
			}

			foreach (var mpt in GKManager.DeviceConfiguration.MPTs.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var mptDescriptor = new MPTDescriptor(this, mpt, DatabaseType.Kau);
				Descriptors.Add(mptDescriptor);

				var mptCreator = new MPTCreator(mpt);
			}

			foreach (var code in GKManager.DeviceConfiguration.Codes.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var codeDescriptor = new CodeDescriptor(code, DatabaseType.Kau);
				Descriptors.Add(codeDescriptor);
			}

			ushort no = 1;
			foreach (var descriptor in Descriptors)
			{
				descriptor.GKBase.KAUDescriptorNo = no++;
				descriptor.DatabaseType = DatabaseType.Kau;
			}
			foreach (var descriptor in Descriptors)
			{
				descriptor.Build();
				if (!descriptor.Formula.IsGeneratedOutside)
				{
					descriptor.BuildFormula();
				}
				descriptor.Formula.Resolve(DatabaseType);
				descriptor.FormulaBytes = descriptor.Formula.GetBytes();
				descriptor.InitializeAllBytes();
			}
		}
	}
}