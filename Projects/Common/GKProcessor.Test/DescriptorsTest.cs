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

		void CreateConfiguration()
		{
			GKDriversCreator.Create();
			var systemDevice = GKManager.DeviceConfiguration.RootDevice = new GKDevice() { DriverUID = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System).UID };
			gkDevice = GKManager.AddChild(systemDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.GK), 0);
			kauDevice1 = GKManager.AddChild(gkDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 1);
			kauDevice2 = GKManager.AddChild(gkDevice, null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU), 2);
		}

		[TestMethod]
		public void TestZoneOnKau()
		{
			CreateConfiguration();

			var device1 = GKManager.AddChild(kauDevice1.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_HandDetector), 1);
			var zone1 = new GKZone();
			GKManager.Zones.Add(zone1);
			device1.ZoneUIDs.Add(zone1.UID);
			
			DescriptorsManager.Create();
			Assert.IsTrue(DescriptorsManager.GkDatabases.Count == 1);
			Assert.IsTrue(DescriptorsManager.KauDatabases.Count == 2);
			var kau1Database = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice1);
			Assert.IsNotNull(kau1Database);
			var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice == gkDevice);
			Assert.IsNotNull(kau1Database);

			var kau1ZoneDescriptor = kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == zone1);
			Assert.IsNotNull(kau1ZoneDescriptor);
			Assert.IsTrue(kau1ZoneDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика зоны на КАУ");

			var gkZoneDescriptor = gkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == zone1);
			Assert.IsNotNull(gkZoneDescriptor);
			Assert.IsTrue(gkZoneDescriptor.Formula.FormulaOperations.Count == 1, "Логика зоны на ГК не пустая");
		}

		[TestMethod]
		public void TestZoneOnGK()
		{
			CreateConfiguration();

			var device1 = GKManager.AddChild(kauDevice1.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_HandDetector), 1);
			var device2 = GKManager.AddChild(kauDevice2.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_HandDetector), 1);
			var zone1 = new GKZone();
			GKManager.Zones.Add(zone1);
			device1.ZoneUIDs.Add(zone1.UID);
			device2.ZoneUIDs.Add(zone1.UID);

			DescriptorsManager.Create();
			var kau1Database = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice1);
			var kau2Database = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice2);
			var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice == gkDevice);

			Assert.IsNull(kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == zone1), "Зона находится в КАУ 1");
			Assert.IsNull(kau2Database.Descriptors.FirstOrDefault(x => x.GKBase == zone1), "Зона находится в КАУ 2");
			var gkZoneDescriptor = gkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == zone1);
			Assert.IsNotNull(gkZoneDescriptor, "Зона не находится в ГК");
			Assert.IsTrue(gkZoneDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика зоны");
		}

		[TestMethod]
		public void TestDeviceLogicOnKau()
		{
			CreateConfiguration();
			var device1 = GKManager.AddChild(kauDevice1.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_HandDetector), 1);
			var device2 = GKManager.AddChild(kauDevice1.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_RM_1), 2);
			var clause = new GKClause();
			clause.ClauseOperationType = ClauseOperationType.AllDevices;
			clause.DeviceUIDs.Add(device1.UID);
			device2.Logic.OnClausesGroup.Clauses.Add(clause);

			DescriptorsManager.Create();
			var kau1Database = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice1);
			var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice == gkDevice);

			var kau1DeviceDescriptor = kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device2);
			var gkDeviceDescriptor = gkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device2);

			Assert.IsTrue(kau1DeviceDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на КАУ");
			Assert.IsTrue(gkDeviceDescriptor.Formula.FormulaOperations.Count == 1, "Присутствует логика на ГК");
		}

		[TestMethod]
		public void TestDeviceLogicOnGK()
		{
			CreateConfiguration();
			var device1 = GKManager.AddChild(kauDevice1.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_HandDetector), 0);
			var device2 = GKManager.AddChild(kauDevice2.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_HandDetector), 0);
			var device3 = GKManager.AddChild(kauDevice1.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_RM_1), 0);
			var clause = new GKClause();
			clause.ClauseOperationType = ClauseOperationType.AllDevices;
			clause.StateType = GKStateBit.Failure;
			clause.DeviceUIDs.Add(device1.UID);
			clause.DeviceUIDs.Add(device2.UID);
			device3.Logic.OnClausesGroup.Clauses.Add(clause);

			DescriptorsManager.Create();
			var kau1Database = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice1);
			var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice == gkDevice);

			var kau1DeviceDescriptor = kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == device3);
			var gkDeviceDescriptor = gkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == device3);

			Assert.IsTrue(kau1DeviceDescriptor.Formula.FormulaOperations.Count == 1, "Присутствует логика на КАУ");
			Assert.IsTrue(gkDeviceDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на ГК");
		}

		[TestMethod]
		public void TestPumpStationOnKau()
		{
			CreateConfiguration();
			var device1 = GKManager.AddChild(kauDevice1.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_HandDetector), 0);
			var device2 = GKManager.AddChild(kauDevice1.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_Bush_Fire), 0);
			var device3 = GKManager.AddChild(kauDevice1.Children[1], null, GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_Bush_Fire), 0);
			var clause = new GKClause();
			clause.ClauseOperationType = ClauseOperationType.AllDevices;
			clause.StateType = GKStateBit.Failure;
			clause.DeviceUIDs.Add(device1.UID);
			var pumpStation = new GKPumpStation();
			pumpStation.StartLogic.OnClausesGroup.Clauses.Add(clause);
			pumpStation.NSDevices.Add(device2);
			pumpStation.NSDevices.Add(device3);
			GKManager.PumpStations.Add(pumpStation);

			DescriptorsManager.Create();
			var kau1Database = DescriptorsManager.KauDatabases.FirstOrDefault(x => x.RootDevice == kauDevice1);
			var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice == gkDevice);

			var kau1PumpStationDescriptor = kau1Database.Descriptors.FirstOrDefault(x => x.GKBase == pumpStation);
			var gkPumpStationDescriptor = gkDatabase.Descriptors.FirstOrDefault(x => x.GKBase == pumpStation);

			Assert.IsTrue(kau1PumpStationDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на КАУ");
			Assert.IsTrue(gkPumpStationDescriptor.Formula.FormulaOperations.Count == 1, "Присутствует логика на ГК");
		}
	}
}