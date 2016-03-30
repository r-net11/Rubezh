using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhClient;
using RubezhAPI.GK;
using System.Collections.Generic;
using RubezhAPI;

namespace GKProcessor.Test
{
	[TestClass]
	public partial class DescriptorsTest
	{
		GKDevice gkDevice;
		GKDevice kauDevice1;
		GKDevice kauDevice2;

		GkDatabase GkDatabase;
		KauDatabase Kau1Database;
		KauDatabase Kau2Database;

		[TestInitialize]
		public void CreateConfiguration()
		{
			GKManager.DeviceConfiguration = new GKDeviceConfiguration();
			GKDriversCreator.Create();
			var systemDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
			Assert.IsNotNull(systemDriver);
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice { Driver = systemDriver, DriverUID = systemDriver.UID };
			gkDevice = GKManager.AddDevice(systemDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice1 = GKManager.AddDevice(gkDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice2 = GKManager.AddDevice(gkDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);
		}

		GKDevice AddDevice(GKDevice device, GKDriverType driverType)
		{
			return GKManager.AddDevice(device.Children[1], GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType), 0);
		}

		void Compile()
		{
			DescriptorsManager.Create();
			Assert.IsTrue(DescriptorsManager.GkDatabases.Count == 1);
			Assert.IsTrue(DescriptorsManager.KauDatabases.Count == 2);
			GkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice == gkDevice);
			Assert.IsNotNull(GkDatabase);
			Kau1Database = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice1);
			Assert.IsNotNull(Kau1Database);
			Kau2Database = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice2);
			Assert.IsNotNull(Kau2Database);
			var descriptorErrors = DescriptorsManager.Check().ToList();
			Assert.IsTrue(!descriptorErrors.Any());
		}

		void CheckDeviceLogicOnGK(GKDevice device)
		{
			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			var deviceKau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика устройства");
			Assert.IsTrue(deviceKau1Descriptor.Formula.FormulaOperations.Count == 1, "На КАУ должна отсутствовать логика устройства");
		}

		void CheckDeviceLogicOnKau(GKDevice device, int kauNo = 1)
		{
			KauDatabase kauDatabase = Kau1Database;
			if (kauNo == 2)
				kauDatabase = Kau2Database;

			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			var deviceKauDescriptor = kauDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count == 1, "На ГК должна отсутствовать логика устройства");
			Assert.IsTrue(deviceKauDescriptor.Formula.FormulaOperations.Count > 1, "На КАУ должна присутствовать логика устройства");
		}

		void CheckObjectLogicOnGK(GKBase gkBase)
		{
			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика объекта");
			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == gkBase), "На КАУ должен отсутствовать объект");
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == gkBase), "На КАУ должен отсутствовать объект");
		}

		void CheckObjectLogicOnKau(GKBase gkBase, int kauNo = 1)
		{
			KauDatabase kauDatabase = Kau1Database;
			if (kauNo == 2)
				kauDatabase = Kau2Database;

			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			var deviceKauDescriptor = kauDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count == 1, "На ГК должна отсутствовать логика объекта");
			Assert.IsTrue(deviceKauDescriptor.Formula.FormulaOperations.Count > 1, "На КАУ должна присутствовать логика объекта");
		}

		[TestMethod]
		public void TestZoneOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var zone1 = new GKZone();
			GKManager.Zones.Add(zone1);
			device1.ZoneUIDs.Add(zone1.UID);
			Compile();

			CheckObjectLogicOnKau(zone1);
		}

		[TestMethod]
		public void TestZoneOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var zone1 = new GKZone();
			GKManager.Zones.Add(zone1);
			device1.ZoneUIDs.Add(zone1.UID);
			device2.ZoneUIDs.Add(zone1.UID);
			Compile();

			CheckObjectLogicOnGK(zone1);
		}

		[TestMethod]
		public void TestDeviceLogicOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { device1.UID }
			};
			device2.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckDeviceLogicOnKau(device2);
		}

		[TestMethod]
		public void TestDeviceLogicOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var device3 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID, device2.UID }
			};
			device3.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckDeviceLogicOnGK(device3);
		}

		[TestMethod]
		public void TestAMPLogicInZoneOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_MAP4);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_MAP4);
			var zone = new GKZone();
			GKManager.Zones.Add(zone);
			device1.ZoneUIDs.Add(zone.UID);
			device2.ZoneUIDs.Add(zone.UID);
			Compile();
			CheckDeviceLogicOnGK(device1);
		}

		[TestMethod]
		public void TestAMPLogicInZoneOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_MAP4);
			var zone = new GKZone();
			GKManager.Zones.Add(zone);
			device1.ZoneUIDs.Add(zone.UID);
			Compile();
			CheckDeviceLogicOnKau(device1);
		}

		[TestMethod]
		public void TestDirectionLogicOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var direction = new GKDirection();
			GKManager.Directions.Add(direction);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { device1.UID }
			};
			direction.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckObjectLogicOnKau(direction);
		}

		[TestMethod]
		public void TestDirectionLogicOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var direction = new GKDirection();
			GKManager.Directions.Add(direction);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID, device2.UID }
			};
			direction.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckObjectLogicOnGK(direction);
		}

		[TestMethod]
		public void TestDelayLogicOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var delay = new GKDelay();
			GKManager.Delays.Add(delay);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { device1.UID }
			};
			delay.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckObjectLogicOnKau(delay);
		}

		[TestMethod]
		public void TestDelayLogicOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var delay = new GKDelay();
			GKManager.Delays.Add(delay);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID, device2.UID }
			};
			delay.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckObjectLogicOnGK(delay);
		}

		[TestMethod]
		public void TestEmptyObjects()
		{
			var zone = new GKZone();
			GKManager.Zones.Add(zone);

			var direction = new GKDirection();
			GKManager.Directions.Add(direction);

			var delay = new GKDelay();
			GKManager.Delays.Add(delay);

			var guardZone = new GKGuardZone();
			GKManager.GuardZones.Add(guardZone);

			var pumpStation = new GKPumpStation();
			GKManager.PumpStations.Add(pumpStation);

			var mpt = new GKMPT();
			GKManager.MPTs.Add(mpt);

			var door = new GKDoor();
			GKManager.Doors.Add(door);

			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			Compile();

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == zone));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == zone));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == zone));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == direction));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == direction));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == direction));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == delay));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == delay));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == delay));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == guardZone));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == guardZone));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == guardZone));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == pumpStation));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == pumpStation));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == pumpStation));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == mpt));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == mpt));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == mpt));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == door));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == door));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == door));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == code));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == code));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == code));
		}

		[TestMethod]
		public void TestEmptyObjectsCrossReferences()
		{
			var direction1 = new GKDirection();
			GKManager.Directions.Add(direction1);

			var direction2 = new GKDirection();
			GKManager.Directions.Add(direction2);

			direction1.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction1.UID } });
			direction2.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction2.UID } });
			Compile();

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == direction1));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == direction1));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == direction1));

			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == direction2));
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == direction2));
			Assert.IsNull(GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == direction2));
		}

		[TestMethod]
		public void TestDependencyChainToKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);

			var direction1 = new GKDirection();
			direction1.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.Fire1, DeviceUIDs = { device1.UID } });
			GKManager.Directions.Add(direction1);

			var direction2 = new GKDirection();
			direction2.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction1.UID } });
			GKManager.Directions.Add(direction2);

			var direction3 = new GKDirection();
			direction3.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction2.UID } });
			GKManager.Directions.Add(direction3);

			var direction4 = new GKDirection();
			direction4.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction3.UID } });
			GKManager.Directions.Add(direction4);

			var direction5 = new GKDirection();
			direction5.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction4.UID } });
			GKManager.Directions.Add(direction5);
			Compile();

			CheckObjectLogicOnKau(direction1);
			CheckObjectLogicOnKau(direction2);
			CheckObjectLogicOnKau(direction3);
			CheckObjectLogicOnKau(direction4);
			CheckObjectLogicOnKau(direction5);
		}

		[TestMethod]
		public void TestDependencyChainToGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);

			var direction1 = new GKDirection();
			direction1.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.Fire1, DeviceUIDs = { device1.UID, device2.UID } });
			GKManager.Directions.Add(direction1);

			var direction2 = new GKDirection();
			direction2.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction1.UID } });
			GKManager.Directions.Add(direction2);

			var direction3 = new GKDirection();
			direction3.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction2.UID } });
			GKManager.Directions.Add(direction3);

			var direction4 = new GKDirection();
			direction4.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction3.UID } });
			GKManager.Directions.Add(direction4);

			var direction5 = new GKDirection();
			direction5.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDirections, StateType = GKStateBit.On, DirectionUIDs = { direction4.UID } });
			GKManager.Directions.Add(direction5);
			Compile();

			CheckObjectLogicOnGK(direction1);
			CheckObjectLogicOnGK(direction2);
			CheckObjectLogicOnGK(direction3);
			CheckObjectLogicOnGK(direction4);
			CheckObjectLogicOnGK(direction5);
		}

		[TestMethod]
		public void TestDependencyToKau()
		{
			var delay1 = new GKDelay();
			delay1.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.Failure, DeviceUIDs = { kauDevice1.UID } });
			GKManager.Delays.Add(delay1);
			Compile();

			CheckObjectLogicOnKau(delay1);
		}

		[TestMethod]
		public void TestDependencyToGK()
		{
			var delay1 = new GKDelay();
			delay1.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.On, DeviceUIDs = { gkDevice.AllChildren.FirstOrDefault(x => x.DriverType == GKDriverType.GKIndicator).UID } });
			GKManager.Delays.Add(delay1);
			Compile();

			CheckObjectLogicOnGK(delay1);
		}

		[TestMethod]
		public void TestDependencyToGKAndKau()
		{
			var delay1 = new GKDelay();
			delay1.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.Failure, DeviceUIDs = { kauDevice1.UID } });
			delay1.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.On, DeviceUIDs = { gkDevice.AllChildren.FirstOrDefault(x => x.DriverType == GKDriverType.GKIndicator).UID } });
			GKManager.Delays.Add(delay1);
			Compile();

			CheckObjectLogicOnGK(delay1);
		}

		[TestMethod]
		public void TestKauCrossReference1()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			device1.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.Fire2, DeviceUIDs = { device2.UID } });
			Compile();
			CheckDeviceLogicOnGK(device1);
		}

		[TestMethod]
		public void TestKauCrossReference2()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var zone = new GKZone();
			GKManager.Zones.Add(zone);
			device2.ZoneUIDs = new List<Guid>() {zone.UID};
			device1.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllZones, StateType = GKStateBit.Fire2, ZoneUIDs = { zone.UID } });
			Compile();
			CheckDeviceLogicOnGK(device1);
			CheckObjectLogicOnKau(zone, 2);
		}

		[TestMethod]
		public void TestKauCrossReference3()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var gkReleDeice = gkDevice.AllChildren.FirstOrDefault(x => x.DriverType == GKDriverType.GKRele);
			gkReleDeice.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.Fire2, DeviceUIDs = { device1.UID } });
			Compile();
			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkReleDeice);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика устройства");
		}

		[TestMethod]
		public void TestKauCrossReference4()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var zone = new GKZone();
			GKManager.Zones.Add(zone);
			device1.ZoneUIDs = new List<Guid>() { zone.UID };
			var gkReleDeice = gkDevice.AllChildren.FirstOrDefault(x => x.DriverType == GKDriverType.GKRele);
			gkReleDeice.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.Fire2, DeviceUIDs = { device1.UID } });
			Compile();
			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkReleDeice);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика устройства");
			CheckObjectLogicOnKau(zone);
		}

		[TestMethod]
		public void TestGKLogic1()
		{
			var gkIndicatorDeice = gkDevice.AllChildren.FirstOrDefault(x => x.DriverType == GKDriverType.GKIndicator);
			var gkReleDeice = gkDevice.AllChildren.FirstOrDefault(x => x.DriverType == GKDriverType.GKRele);
			gkReleDeice.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.On, DeviceUIDs = { gkIndicatorDeice.UID } });
			Compile();
			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkReleDeice);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика устройства");
		}

		[TestMethod]
		public void TestGKLogic2()
		{
			var kauIndicatorDeice = kauDevice1.Children.FirstOrDefault(x => x.DriverType == GKDriverType.KAUIndicator);
			var gkReleDeice = gkDevice.AllChildren.FirstOrDefault(x => x.DriverType == GKDriverType.GKRele);
			gkReleDeice.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.On, DeviceUIDs = { kauIndicatorDeice.UID } });
			Compile();
			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkReleDeice);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика устройства");
		}

		[TestMethod]
		public void TestGKLogic3()
		{
			var gkReleDeice = gkDevice.AllChildren.FirstOrDefault(x => x.DriverType == GKDriverType.GKRele);
			gkReleDeice.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.On, DeviceUIDs = { kauDevice1.UID } });
			Compile();
			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkReleDeice);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика устройства");
		}

		[TestMethod]
		public void TestGKLogic4()
		{
			var gkReleDeice = gkDevice.AllChildren.FirstOrDefault(x => x.DriverType == GKDriverType.GKRele);
			gkReleDeice.Logic.OnClausesGroup.Clauses.Add(new GKClause() { ClauseOperationType = ClauseOperationType.AllDevices, StateType = GKStateBit.On, DeviceUIDs = { kauDevice1.UID, kauDevice2.UID } });
			Compile();
			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkReleDeice);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика устройства");
		}
	}
}