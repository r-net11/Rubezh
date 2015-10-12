using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhClient;
using FiresecAPI.GK;

namespace GKProcessor.Test
{
	public partial class DescriptorsTest
	{
		[TestMethod]
		public void TestPumpStationOnKau()
		{
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

			var kau1PimDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase is GKPim && (x.GKBase as GKPim).PumpStationUID == pumpStation.UID);
			var gkPimDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase is GKPim && (x.GKBase as GKPim).PumpStationUID == pumpStation.UID);
			Assert.IsTrue(kau1PimDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на КАУ");
			Assert.IsTrue(gkPimDescriptor.Formula.FormulaOperations.Count == 1, "Присутствует логика на ГК");

			CheckDeviceLogicOnKau(device2);
		}

		[TestMethod]
		public void TestPumpStationOnGK()
		{
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

			var kau1PimDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase is GKPim && (x.GKBase as GKPim).PumpStationUID == pumpStation.UID);
			var gkPimDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase is GKPim && (x.GKBase as GKPim).PumpStationUID == pumpStation.UID);
			Assert.IsNull(kau1PimDescriptor, "На КАУ присутствует компонент НС");
			Assert.IsTrue(gkPimDescriptor.Formula.FormulaOperations.Count > 1, "Отсутствует логика на ГК");

			CheckDeviceLogicOnGK(device2);
		}
	}
}