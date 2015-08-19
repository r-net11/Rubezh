using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FiresecClient;
using FiresecAPI.GK;

namespace GKProcessor.Test
{
	[TestClass]
	public class DescriptorsTest
	{
		GKDevice gkDevice;
		GKDevice kauDevice1;
		GKDevice kauDevice2;

		GkDatabase GkDatabase;
		KauDatabase Kau1Database;
		KauDatabase Kau2Database;

		void CreateConfiguration()
		{
			GKDriversCreator.Create();
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice() { DriverUID = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System).UID };
			gkDevice = GKManager.AddChild(systemDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice1 = GKManager.AddChild(gkDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice2 = GKManager.AddChild(gkDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);
		}

		GKDevice AddDevice(GKDevice device, GKDriverType driverType)
		{
			return GKManager.AddChild(device.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType), 0);
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
			Assert.IsTrue(DescriptorsManager.Check());
		}

		void CheckDeviceLogicOnGK(GKDevice device)
		{
			var device1GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			var device1Kau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device);
			Assert.IsTrue(device1GKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика устройства");
			Assert.IsTrue(device1Kau1Descriptor.Formula.FormulaOperations.Count == 1, "На КАУ должна отсутствовать логика устройства");
		}

		void CheckDeviceLogicOnKau(GKDevice device)
		{
			var device1GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			var device1Kau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device);
			Assert.IsTrue(device1GKDescriptor.Formula.FormulaOperations.Count == 1, "На ГК должна отсутствовать логика устройства");
			Assert.IsTrue(device1Kau1Descriptor.Formula.FormulaOperations.Count > 1, "На КАУ должна присутствовать логика устройства");
		}

		void CheckObjectLogicOnGK(GKBase gkBase)
		{
			var device1GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			Assert.IsTrue(device1GKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика объекта");
			Assert.IsNull(Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == gkBase), "На КАУ должен отсутствовать объект");
			Assert.IsNull(Kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == gkBase), "На КАУ должен отсутствовать объект");
		}

		void CheckObjectLogicOnKau(GKBase gkBase)
		{
			var device1GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			var device1Kau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			Assert.IsTrue(device1GKDescriptor.Formula.FormulaOperations.Count == 1, "На ГК должна отсутствовать логика объекта");
			Assert.IsTrue(device1Kau1Descriptor.Formula.FormulaOperations.Count > 1, "На КАУ должна присутствовать логика объекта");
		}

		[TestMethod]
		public void TestZoneOnKau()
		{
			CreateConfiguration();
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
			CreateConfiguration();
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
			CreateConfiguration();
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				DeviceUIDs = { device1.UID }
			};
			device2.Logic.OnClausesGroup.Clauses.Add(clause);
			Compile();

			CheckObjectLogicOnKau(device2);
		}

		[TestMethod]
		public void TestDeviceLogicOnGK()
		{
			CreateConfiguration();
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

			var gkDeviceDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device3);
			var kau1DeviceDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device3);

			Assert.IsTrue(kau1DeviceDescriptor.Formula.FormulaOperations.Count == 1, "Присутствует логика на КАУ");
			Assert.IsTrue(gkDeviceDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на ГК");
		}

		[TestMethod]
		public void TestPumpStationOnKau()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_Bush_Fire);
			var device3 = AddDevice(kauDevice1, GKDriverType.RSR2_Bush_Fire);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			var pumpStation = new GKPumpStation()
			{
				NSDeviceUIDs = { device2.UID, device3.UID }
			};
			pumpStation.StartLogic.OnClausesGroup.Clauses.Add(clause);
			GKManager.PumpStations.Add(pumpStation);
			Compile();

			CheckObjectLogicOnKau(pumpStation);

			var kau1DelayDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase is GKDelay && (x.GKBase as GKDelay).PumpStationUID == pumpStation.UID);
			var gkDelayDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase is GKDelay && (x.GKBase as GKDelay).PumpStationUID == pumpStation.UID);
			Assert.IsTrue(kau1DelayDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на КАУ");
			Assert.IsTrue(gkDelayDescriptor.Formula.FormulaOperations.Count == 1, "Присутствует логика на ГК");

			var kau1MainDelayDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase is GKDelay && (x.GKBase as GKDelay).PumpStationUID == Guid.Empty);
			var gkMainDelayDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase is GKDelay && (x.GKBase as GKDelay).PumpStationUID == Guid.Empty);
			Assert.IsTrue(kau1MainDelayDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на КАУ");
			Assert.IsTrue(gkMainDelayDescriptor.Formula.FormulaOperations.Count == 1, "Присутствует логика на ГК");

			var kau1PimDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase is GKPim && (x.GKBase as GKPim).PumpStationUID == pumpStation.UID);
			var gkPimDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase is GKPim && (x.GKBase as GKPim).PumpStationUID == pumpStation.UID);
			Assert.IsTrue(kau1PimDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на КАУ");
			Assert.IsTrue(gkPimDescriptor.Formula.FormulaOperations.Count == 1, "Присутствует логика на ГК");

			CheckDeviceLogicOnKau(device2);
		}

		[TestMethod]
		public void TestPumpStationOnGK()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_Bush_Fire);
			var device3 = AddDevice(kauDevice1, GKDriverType.RSR2_Bush_Fire);
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure
			};
			clause.DeviceUIDs.Add(device1.UID);
			var pumpStation = new GKPumpStation()
			{
				NSDeviceUIDs = { device2.UID, device3.UID }
			};
			pumpStation.StartLogic.OnClausesGroup.Clauses.Add(clause);
			GKManager.PumpStations.Add(pumpStation);
			Compile();

			CheckObjectLogicOnGK(pumpStation);

			var kau1DelayDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase is GKDelay && (x.GKBase as GKDelay).PumpStationUID == pumpStation.UID);
			var gkDelayDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase is GKDelay && (x.GKBase as GKDelay).PumpStationUID == pumpStation.UID);
			Assert.IsNull(kau1DelayDescriptor, "На КАУ присутствует компонент НС");
			Assert.IsTrue(gkDelayDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на ГК");

			var kau1MainDelayDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase is GKDelay && (x.GKBase as GKDelay).PumpStationUID == Guid.Empty);
			var gkMainDelayDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase is GKDelay && (x.GKBase as GKDelay).PumpStationUID == Guid.Empty);
			Assert.IsNull(kau1MainDelayDescriptor, "На КАУ присутствует компонент НС");
			Assert.IsTrue(gkMainDelayDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на ГК");

			var kau1PimDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase is GKPim && (x.GKBase as GKPim).PumpStationUID == pumpStation.UID);
			var gkPimDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase is GKPim && (x.GKBase as GKPim).PumpStationUID == pumpStation.UID);
			Assert.IsNull(kau1PimDescriptor, "На КАУ присутствует компонент НС");
			Assert.IsTrue(gkPimDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на ГК");

			CheckDeviceLogicOnGK(device2);
		}

		[TestMethod]
		public void TestMPTOnKau()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var mpt = new GKMPT();
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			mpt.MPTDevices.Add(new GKMPTDevice() { MPTDeviceType = GKMPTDeviceType.Bomb, DeviceUID = device2.UID });
			GKManager.MPTs.Add(mpt);
			Compile();

			CheckObjectLogicOnKau(mpt);
			CheckDeviceLogicOnKau(device2);
		}

		[TestMethod]
		public void TestMPTOnGK()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice2, GKDriverType.RSR2_HandDetector);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var mpt = new GKMPT();
			var clause = new GKClause()
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { device1.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			mpt.MPTDevices.Add(new GKMPTDevice() { MPTDeviceType = GKMPTDeviceType.Bomb, DeviceUID = device2.UID });
			GKManager.MPTs.Add(mpt);
			Compile();

			CheckObjectLogicOnGK(mpt);
			CheckDeviceLogicOnGK(device2);
		}

		[TestMethod]
		public void TestDoorOnGK()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_CodeReader);
			var device3 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var device4 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.TwoWay,
				EnterDeviceUID = device1.UID,
				ExitDeviceUID = device2.UID,
				LockDeviceUID = device3.UID,
				LockControlDeviceUID = device4.UID
			};
			GKManager.Doors.Add(door);
			Compile();

			var device1GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device1);
			var device1Kau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device1);
			//Assert.IsTrue(device1GKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика ТД");
			Assert.IsTrue(device1Kau1Descriptor.Formula.FormulaOperations.Count == 1, "На КАУ должна отсутствовать логика ТД");

			var device2GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device2);
			var device2Kau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device2);
			//Assert.IsTrue(device2GKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика ТД");
			Assert.IsTrue(device2Kau1Descriptor.Formula.FormulaOperations.Count == 1, "На КАУ должна отсутствовать логика ТД");

			var device3GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device3);
			var device3Kau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device3);
			Assert.IsTrue(device3GKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика ТД");
			Assert.IsTrue(device3Kau1Descriptor.Formula.FormulaOperations.Count == 1, "На КАУ должна отсутствовать логика ТД");

			var device4GKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device4);
			var device4Kau1Descriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device4);
			//Assert.IsTrue(device4GKDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика ТД");
			Assert.IsTrue(device4Kau1Descriptor.Formula.FormulaOperations.Count == 1, "На КАУ должна отсутствовать логика ТД");
		}

		[TestMethod]
		public void TestGuardZoneOnKau()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.SetGuard, DeviceUID = device1.UID });
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.ResetGuard, DeviceUID = device2.UID });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnKau(guardZone);
		}

		[TestMethod]
		public void TestGuardZoneOnGK()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_AM_1);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.SetGuard, DeviceUID = device1.UID });
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.ResetGuard, DeviceUID = device2.UID });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnGK(guardZone);
		}

		[TestMethod]
		public void TestGuardZoneWithLevelsOnKau()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_CodeReader);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device1.UID, CodeReaderSettings = new GKCodeReaderSettings() { SetGuardSettings = new GKCodeReaderSettingsPart() { CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne, AccessLevel = 1 } } });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnGK(guardZone);
			CheckDeviceLogicOnGK(device1);
		}

		[TestMethod]
		public void TestGuardZoneWithCodesOnKau()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_CodeReader);
			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device1.UID, CodeReaderSettings = new GKCodeReaderSettings() { SetGuardSettings = new GKCodeReaderSettingsPart() { CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne, CodeUIDs = { code.UID } } } });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnKau(guardZone);
			CheckDeviceLogicOnKau(device1);
		}

		[TestMethod]
		public void TestGuardZoneWithCodesAndLevelsOnGK()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_CodeReader);
			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device1.UID, CodeReaderSettings = new GKCodeReaderSettings() { SetGuardSettings = new GKCodeReaderSettingsPart() { CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne, CodeUIDs = { code.UID }, AccessLevel = 1 } } });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnGK(guardZone);
			CheckDeviceLogicOnGK(device1);
		}

		public void TestGuardZoneWithChangeAmOnKau()
		{
			CreateConfiguration();
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device1.UID, ActionType = GKGuardZoneDeviceActionType.ChangeGuard });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnKau(guardZone);
			CheckDeviceLogicOnKau(device1);

			var pimGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase is GKPim);
			var pimKauDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase is GKPim);
			Assert.IsTrue(pimGKDescriptor.Formula.FormulaOperations.Count == 1, "На ГК должна отсутствовать логика ПИМ");
			Assert.IsTrue(pimKauDescriptor.Formula.FormulaOperations.Count > 1, "На КАУ должна присутствовать логика ПИМ");
		}
	}
}