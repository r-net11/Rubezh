using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace GKProcessor.Test
{
	[TestClass]
	public partial class MirrorsTest
	{
		GKDevice gkDevice;
		GKDevice kauDevice;
		GKDevice reflectionDevice1;
		GKDevice reflectionDevice2;

		GkDatabase GkDatabase;
		KauDatabase KauDatabase;

		[TestInitialize]
		public void CreateConfiguration()
		{
			var systemDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
			Assert.IsNotNull(systemDriver);
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice { Driver = systemDriver, DriverUID = systemDriver.UID };
			gkDevice = GKManager.AddDevice(systemDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice = GKManager.AddDevice(gkDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			reflectionDevice1 = GKManager.AddDevice(gkDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GKMirror), 2);
			reflectionDevice2 = GKManager.AddDevice(gkDevice, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GKMirror), 3);
		}

		GKDevice AddDevice(GKDevice device, GKDriverType driverType)
		{
			return GKManager.AddDevice(device.Children[1], GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType), 0);
		}

		void Compile()
		{
			DescriptorsManager.Create();
			GkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice == gkDevice);
			Assert.IsNotNull(GkDatabase);
			KauDatabase = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice);
			Assert.IsNotNull(KauDatabase);
			var descriptorErrors = DescriptorsManager.Check().ToList();
			Assert.IsTrue(!descriptorErrors.Any());
		}
		void CheckDeviceLogicOnGK(GKDevice device)
		{
			var gkBaseGkDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			var gkBaseKauDescriptor = KauDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			Assert.IsTrue(gkBaseGkDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика объекта");
			Assert.IsTrue(gkBaseKauDescriptor.Formula.FormulaOperations.Count == 1, "На КАУ должна отсутствовать логика объекта");
		}

		void CheckDeviceHasNoLogic(GKDevice device)
		{
			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			var deviceKauDescriptor = KauDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count == 1, "На ГК должна отсутствовать логика устройства");
			Assert.IsTrue(deviceKauDescriptor.Formula.FormulaOperations.Count == 1, "На КАУ должна отсутствовать логика устройства");
		}

		void CheckObjectOnGK(GKBase gkBase)
		{
			var gkBaseGkDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			var gkBaseKauDescriptor = KauDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			Assert.IsNotNull(gkBaseGkDescriptor);
			Assert.IsNull(gkBaseKauDescriptor);
		}

		void Test(GKBase gkBase, GKDriverType mirrorType)
		{
			TestOneMirror(gkBase, mirrorType);
			TestTwoMirrors(gkBase, mirrorType);
			TestTwoReflections(gkBase, mirrorType);
		}

		void TestFirefightingZone(GKZone zone, GKDirection direction)
		{
			TestOneFirefightingZoneMirror(zone, direction);
			TestTwoFirefightingZoneMirrors(zone, direction);
			TestTwoFirefightingZoneReflections(zone, direction);
		}

		void TestOneMirror(GKBase gkBase, GKDriverType mirrorType)
		{
			var mirrorDevice = AddDevice(reflectionDevice1, mirrorType);
			GKManager.AddToMirror(gkBase, mirrorDevice);
			Compile();
			Assert.IsTrue(gkBase.OutputDescriptors.Contains(mirrorDevice));
			if (mirrorType != GKDriverType.DetectorDevicesMirror)
			{
				Assert.IsTrue(gkBase.InputDescriptors.Contains(mirrorDevice));
				var rmDevice = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
				rmDevice.Logic.OnClausesGroup.Clauses.Add(GetClause(gkBase));
				Compile();
				CheckDeviceLogicOnGK(rmDevice);
			}
		}

		void TestTwoMirrors(GKBase gkBase, GKDriverType mirrorType)
		{
			var mirrorDevice1 = AddDevice(reflectionDevice1, mirrorType);
			var mirrorDevice2 = AddDevice(reflectionDevice1, mirrorType);
			GKManager.AddToMirror(gkBase, mirrorDevice1);
			GKManager.AddToMirror(gkBase, mirrorDevice2);
			Compile();
			Assert.IsTrue(gkBase.OutputDescriptors.Contains(mirrorDevice1));
			Assert.IsTrue(gkBase.OutputDescriptors.Contains(mirrorDevice2));
			if (mirrorType != GKDriverType.DetectorDevicesMirror)
			{
				Assert.IsTrue(gkBase.InputDescriptors.Contains(mirrorDevice1));
				Assert.IsTrue(gkBase.InputDescriptors.Contains(mirrorDevice2));
				var rmDevice = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
				rmDevice.Logic.OnClausesGroup.Clauses.Add(GetClause(gkBase));
				Compile();
				CheckDeviceLogicOnGK(rmDevice);
			}
		}

		void TestTwoReflections(GKBase gkBase, GKDriverType mirrorType)
		{
			var mirrorDevice1 = AddDevice(reflectionDevice1, mirrorType);
			var mirrorDevice2 = AddDevice(reflectionDevice2, mirrorType);
			GKManager.AddToMirror(gkBase, mirrorDevice1);
			GKManager.AddToMirror(gkBase, mirrorDevice2);
			Compile();
			Assert.IsTrue(gkBase.OutputDescriptors.Contains(mirrorDevice1));
			Assert.IsTrue(gkBase.OutputDescriptors.Contains(mirrorDevice2));
			if (mirrorType != GKDriverType.DetectorDevicesMirror)
			{
				Assert.IsTrue(gkBase.InputDescriptors.Contains(mirrorDevice1));
				Assert.IsTrue(gkBase.InputDescriptors.Contains(mirrorDevice2));
				var rmDevice = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
				rmDevice.Logic.OnClausesGroup.Clauses.Add(GetClause(gkBase));
				Compile();
				CheckDeviceLogicOnGK(rmDevice);
			}
		}

		void TestOneFirefightingZoneMirror(GKZone zone, GKDirection direction)
		{
			var mirrorDevice = AddDevice(reflectionDevice1, GKDriverType.FirefightingZonesMirror);
			GKManager.AddToMirror(zone, mirrorDevice);
			GKManager.AddToMirror(direction, mirrorDevice);
			Compile();
			Assert.IsTrue(zone.OutputDescriptors.Contains(mirrorDevice));
			Assert.IsTrue(zone.InputDescriptors.Contains(mirrorDevice));
			Assert.IsTrue(direction.OutputDescriptors.Contains(mirrorDevice));
			Assert.IsTrue(direction.InputDescriptors.Contains(mirrorDevice));
			var rmDevice1 = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
			rmDevice1.Logic.OnClausesGroup.Clauses.Add(GetClause(zone));
			var rmDevice2 = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
			rmDevice2.Logic.OnClausesGroup.Clauses.Add(GetClause(direction));
			Compile();
			CheckDeviceLogicOnGK(rmDevice1);
			CheckDeviceLogicOnGK(rmDevice2);
		}

		void TestTwoFirefightingZoneMirrors(GKZone zone, GKDirection direction)
		{
			var mirrorDevice1 = AddDevice(reflectionDevice1, GKDriverType.FirefightingZonesMirror);
			var mirrorDevice2 = AddDevice(reflectionDevice1, GKDriverType.FirefightingZonesMirror);
			GKManager.AddToMirror(zone, mirrorDevice1);
			GKManager.AddToMirror(zone, mirrorDevice2);
			GKManager.AddToMirror(direction, mirrorDevice1);
			GKManager.AddToMirror(direction, mirrorDevice2);
			Compile();
			Assert.IsTrue(zone.OutputDescriptors.Contains(mirrorDevice1));
			Assert.IsTrue(zone.OutputDescriptors.Contains(mirrorDevice2));
			Assert.IsTrue(zone.InputDescriptors.Contains(mirrorDevice1));
			Assert.IsTrue(zone.InputDescriptors.Contains(mirrorDevice2));
			Assert.IsTrue(direction.OutputDescriptors.Contains(mirrorDevice1));
			Assert.IsTrue(direction.OutputDescriptors.Contains(mirrorDevice2));
			Assert.IsTrue(direction.InputDescriptors.Contains(mirrorDevice1));
			Assert.IsTrue(direction.InputDescriptors.Contains(mirrorDevice2));
			var rmDevice1 = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
			rmDevice1.Logic.OnClausesGroup.Clauses.Add(GetClause(zone));
			var rmDevice2 = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
			rmDevice2.Logic.OnClausesGroup.Clauses.Add(GetClause(direction));
			Compile();
			CheckDeviceLogicOnGK(rmDevice1);
			CheckDeviceLogicOnGK(rmDevice2);
		}

		void TestTwoFirefightingZoneReflections(GKZone zone, GKDirection direction)
		{
			var mirrorDevice1 = AddDevice(reflectionDevice1, GKDriverType.FirefightingZonesMirror);
			var mirrorDevice2 = AddDevice(reflectionDevice2, GKDriverType.FirefightingZonesMirror);
			GKManager.AddToMirror(zone, mirrorDevice1);
			GKManager.AddToMirror(zone, mirrorDevice2);
			GKManager.AddToMirror(direction, mirrorDevice1);
			GKManager.AddToMirror(direction, mirrorDevice2);
			Compile();
			Assert.IsTrue(zone.OutputDescriptors.Contains(mirrorDevice1));
			Assert.IsTrue(zone.OutputDescriptors.Contains(mirrorDevice2));
			Assert.IsTrue(zone.InputDescriptors.Contains(mirrorDevice1));
			Assert.IsTrue(zone.InputDescriptors.Contains(mirrorDevice2));
			Assert.IsTrue(direction.OutputDescriptors.Contains(mirrorDevice1));
			Assert.IsTrue(direction.OutputDescriptors.Contains(mirrorDevice2));
			Assert.IsTrue(direction.InputDescriptors.Contains(mirrorDevice1));
			Assert.IsTrue(direction.InputDescriptors.Contains(mirrorDevice2));
			var rmDevice1 = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
			rmDevice1.Logic.OnClausesGroup.Clauses.Add(GetClause(zone));
			var rmDevice2 = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
			rmDevice2.Logic.OnClausesGroup.Clauses.Add(GetClause(direction));
			Compile();
			CheckDeviceLogicOnGK(rmDevice1);
			CheckDeviceLogicOnGK(rmDevice2);
		}

		GKClause GetClause(GKBase gkBase)
		{
			var clause = new GKClause();

			if (gkBase is GKDevice)
			{
				clause.ClauseOperationType = ClauseOperationType.AllDevices;
				clause.DeviceUIDs = new List<Guid> { gkBase.UID };
				clause.StateType = GKStateBit.Failure;
			}

			if (gkBase is GKZone)
			{
				clause.ClauseOperationType = ClauseOperationType.AllZones;
				clause.ZoneUIDs = new List<Guid> { gkBase.UID };
				clause.StateType = GKStateBit.Fire1;
			}

			if (gkBase is GKGuardZone)
			{
				clause.ClauseOperationType = ClauseOperationType.AllGuardZones;
				clause.GuardZoneUIDs = new List<Guid> { gkBase.UID };
				clause.StateType = GKStateBit.Attention;
			}

			if (gkBase is GKDirection)
			{
				clause.ClauseOperationType = ClauseOperationType.AllDirections;
				clause.DirectionUIDs = new List<Guid> { gkBase.UID };
				clause.StateType = GKStateBit.On;
			}

			if (gkBase is GKDelay)
			{
				clause.ClauseOperationType = ClauseOperationType.AllDelays;
				clause.DelayUIDs = new List<Guid> { gkBase.UID };
				clause.StateType = GKStateBit.On;
			}

			if (gkBase is GKDoor)
			{
				clause.ClauseOperationType = ClauseOperationType.AllDoors;
				clause.DoorUIDs = new List<Guid> { gkBase.UID };
				clause.StateType = GKStateBit.On;
			}

			if (gkBase is GKMPT)
			{
				clause.ClauseOperationType = ClauseOperationType.AllMPTs;
				clause.MPTUIDs = new List<Guid> { gkBase.UID };
				clause.StateType = GKStateBit.On;
			}

			if (gkBase is GKPumpStation)
			{
				clause.ClauseOperationType = ClauseOperationType.AllPumpStations;
				clause.PumpStationsUIDs = new List<Guid> { gkBase.UID };
				clause.StateType = GKStateBit.On;
			}

			return clause;
		}
	}
}