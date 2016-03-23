using RubezhAPI.GK;
using RubezhAPI;
using System.Collections.Generic;
using System.Linq;

namespace GKProcessor
{
	public class KauDatabase : CommonDatabase
	{
		public KauDatabase(GKDevice kauDevice)
		{
			DatabaseType = kauDevice.DriverType == GKDriverType.GKMirror ? DatabaseType.Mirror : DatabaseType.Kau;
			RootDevice = kauDevice;

			AddChild(RootDevice);
			Devices.ForEach(x => x.KauDatabaseParent = RootDevice);

			GlobalPim = new GKPim { Name = "Автоматика" + "(" + kauDevice.PresentationName + ")", IsGlobalPim = true };
			GlobalPim.DeviceUid = kauDevice.UID;
			GlobalPimDescriptor = new PimDescriptor(GlobalPim);
		}

		void AddChild(GKDevice device)
		{
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
			if (DatabaseType == DatabaseType.Mirror)
				RestructCollection(Devices);
			foreach (var device in Devices)
			{
				if (device.Driver.HasMirror)
					Descriptors.Add(new MirrorDescriptor(device));
				else
					Descriptors.Add(new DeviceDescriptor(device));
			}

			foreach (var zone in GKManager.Zones.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var zoneDescriptor = new ZoneDescriptor(zone);
				Descriptors.Add(zoneDescriptor);
			}

			foreach (var guardZone in GKManager.GuardZones.Where(x => x.KauDatabaseParent == RootDevice && !x.HasAccessLevel))
			{
				var guardZoneDescriptor = new GuardZoneDescriptor(guardZone);
				Descriptors.Add(guardZoneDescriptor);

				if (guardZoneDescriptor.GuardZonePimDescriptor != null)
				{
					Descriptors.Add(guardZoneDescriptor.GuardZonePimDescriptor);
				}
				if (guardZoneDescriptor.GuardZoneChangePimDescriptor != null)
				{
					Descriptors.Add(guardZoneDescriptor.GuardZoneChangePimDescriptor);
				}
			}

			foreach (var direction in GKManager.Directions.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var directionDescriptor = new DirectionDescriptor(direction);
				Descriptors.Add(directionDescriptor);
			}

			foreach (var delay in GKManager.Delays.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var delayDescriptor = new DelayDescriptor(delay);
				Descriptors.Add(delayDescriptor);
			}

			foreach (var pumpStation in GKManager.PumpStations.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var pumpStationDescriptor = new PumpStationDescriptor(GlobalPim, pumpStation);
				Descriptors.Add(pumpStationDescriptor);

				var pumpStationCreator = new PumpStationCreator(this, pumpStation, DatabaseType.Kau);
				pumpStationCreator.Create();
			}

			foreach (var mpt in GKManager.DeviceConfiguration.MPTs.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var mptDescriptor = new MPTDescriptor(GlobalPim, mpt);
				Descriptors.Add(mptDescriptor);

				var mptCreator = new MPTCreator(mpt);
			}

			foreach (var code in GKManager.DeviceConfiguration.Codes.Where(x => x.KauDatabaseParent == RootDevice))
			{
				var codeDescriptor = new CodeDescriptor(code);
				Descriptors.Add(codeDescriptor);
			}

			if (DatabaseType == DatabaseType.Kau)
				Descriptors.Add(GlobalPimDescriptor);

			ushort no = 1;
			foreach (var descriptor in Descriptors)
			{
				descriptor.No = descriptor.GKBase.KAUDescriptorNo = no++;
				descriptor.DatabaseType = RootDevice.DriverType == GKDriverType.GKMirror ? DatabaseType.Mirror : DatabaseType.Kau;
				descriptor.GKBase.KauDatabaseParent = RootDevice; // для автосгенерированных объектов
			}
			foreach (var descriptor in Descriptors)
			{
				descriptor.Build();
				if (!descriptor.IsFormulaGeneratedOutside && descriptor.GKBase.IsLogicOnKau)
				{
					descriptor.BuildFormula();
				}
				descriptor.Formula.Resolve(this);
				descriptor.FormulaBytes = descriptor.Formula.GetBytes();
				if (!(descriptor is MirrorDescriptor))
				{
					descriptor.GKBase.InputDescriptors = descriptor.GKBase.InputDescriptors.OrderBy(x => x.No).ToList();
					descriptor.GKBase.OutputDescriptors = descriptor.GKBase.OutputDescriptors.OrderBy(x => x.No).ToList();
				}
				descriptor.InitializeAllBytes();
			}
		}
	}
}