using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhClient;
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
			GKDriversCreator.Create();
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice() { DriverUID = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System).UID };
			gkDevice = GKManager.AddChild(systemDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice = GKManager.AddChild(gkDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			reflectionDevice1 = GKManager.AddChild(gkDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GKMirror), 2);
			reflectionDevice2 = GKManager.AddChild(gkDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GKMirror), 3);
		}

		GKDevice AddDevice(GKDevice device, GKDriverType driverType)
		{
			return GKManager.AddChild(device.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == driverType), 0);
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
		void CheckObjectLogicOnGK(GKBase gkBase)
		{
			var deviceGkDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			var deviceKauDescriptor = KauDatabase.Descriptors.FirstOrDefault(x => x.GKBase == gkBase);
			Assert.IsTrue(deviceGkDescriptor.Formula.FormulaOperations.Count > 1, "На ГК должна присутствовать логика объекта");
			Assert.IsTrue(deviceKauDescriptor.Formula.FormulaOperations.Count == 1, "На КАУ должна отсутствовать логика объекта");
		}

		void CheckDeviceLogicOnKau(GKDevice device, int kauNo = 1)
		{
			var deviceGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			var deviceKauDescriptor = KauDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device);
			Assert.IsTrue(deviceGKDescriptor.Formula.FormulaOperations.Count == 1, "На ГК должна отсутствовать логика устройства");
			Assert.IsTrue(deviceKauDescriptor.Formula.FormulaOperations.Count > 1, "На КАУ должна присутствовать логика устройства");
		}

		[TestMethod]
		public void TestDetectorDevice()
		{
			var device = AddDevice(kauDevice, GKDriverType.RSR2_SmokeDetector);
			Test(device, GKDriverType.DetectorDevicesMirror);
		}

		[TestMethod]
		public void TestControlDevice()
		{
			var device = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
			Test(device, GKDriverType.ControlDevicesMirror);
		}

		[TestMethod]
		public void TestZone()
		{
			var device = AddDevice(kauDevice, GKDriverType.RSR2_HandDetector);
			var zone = new GKZone();
			GKManager.Zones.Add(zone);
			device.ZoneUIDs.Add(zone.UID);
			Test(zone, GKDriverType.FireZonesMirror);
		}

		[TestMethod]
		public void TestGuardZone()
		{
			var device = AddDevice(kauDevice, GKDriverType.RSR2_GuardDetector);
			var guardZone = new GKGuardZone();
			GKManager.AddGuardZone(guardZone);
			GKManager.AddDeviceToGuardZone(device, guardZone);
			Test(guardZone, GKDriverType.GuardZonesMirror);
			CheckObjectLogicOnGK(device);
		}

		void Test(GKBase gkBase, GKDriverType mirrorType)
		{
			TestOneMirror(gkBase, mirrorType);
			TestTwoMirror(gkBase, mirrorType);
			TestTwoReflection(gkBase, mirrorType);
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
				AddLogic(rmDevice, gkBase);
				Compile();
				CheckObjectLogicOnGK(rmDevice);
			}
		}

		void TestTwoMirror(GKBase gkBase, GKDriverType mirrorType)
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
				AddLogic(rmDevice, gkBase);
				Compile();
				CheckObjectLogicOnGK(rmDevice);
			}
		}

		void TestTwoReflection(GKBase gkBase, GKDriverType mirrorType)
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
				AddLogic(rmDevice, gkBase);
				Compile();
				CheckObjectLogicOnGK(rmDevice);
			}
		}

		void AddLogic(GKDevice targetDevice, GKBase gkBase)
		{
			var clause = new GKClause();

			if (gkBase is GKDevice)
			{
				clause.ClauseOperationType = ClauseOperationType.AllDevices;
				clause.DeviceUIDs = new List<Guid> { gkBase.UID };
			}

			if (gkBase is GKZone)
			{
				clause.ClauseOperationType = ClauseOperationType.AllZones;
				clause.ZoneUIDs = new List<Guid> { gkBase.UID };
			}

			if (gkBase is GKGuardZone)
			{
				clause.ClauseOperationType = ClauseOperationType.AllGuardZones;
				clause.GuardZoneUIDs = new List<Guid> { gkBase.UID };
			}

			if (gkBase is GKDirection)
			{
				clause.ClauseOperationType = ClauseOperationType.AllDirections;
				clause.DirectionUIDs = new List<Guid> { gkBase.UID };
			}

			if (gkBase is GKDelay)
			{
				clause.ClauseOperationType = ClauseOperationType.AllDelays;
				clause.DelayUIDs = new List<Guid> { gkBase.UID };
			}

			if (gkBase is GKDoor)
			{
				clause.ClauseOperationType = ClauseOperationType.AllDoors;
				clause.DoorUIDs = new List<Guid> { gkBase.UID };
			}

			if (gkBase is GKMPT)
			{
				clause.ClauseOperationType = ClauseOperationType.AllMPTs;
				clause.MPTUIDs = new List<Guid> { gkBase.UID };
			}

			if (gkBase is GKMPT)
			{
				clause.ClauseOperationType = ClauseOperationType.AllPumpStations;
				clause.PumpStationsUIDs = new List<Guid> { gkBase.UID };
			}

			targetDevice.Logic.OnClausesGroup.Clauses.Add(clause);
		}
	}
}