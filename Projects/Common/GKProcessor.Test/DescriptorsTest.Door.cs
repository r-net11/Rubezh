using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FiresecClient;
using FiresecAPI.GK;

namespace GKProcessor.Test
{
	public partial class DescriptorsTest
	{
		[TestMethod]
		public void TestDoorOnGK()
		{
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
		public void TestAntipassback1()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var device3 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var device4 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.TwoWay,
				AntipassbackOn = true,
				EnterDeviceUID = device1.UID,
				ExitDeviceUID = device2.UID,
				LockDeviceUID = device3.UID,
				LockControlDeviceUID = device4.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Add(door);
			Compile();
		}

		[TestMethod]
		public void TestAntipassback2()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var device3 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var device4 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var device5 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.Turnstile,
				AntipassbackOn = true,
				EnterDeviceUID = device1.UID,
				ExitDeviceUID = device2.UID,
				LockDeviceUID = device3.UID,
				LockDeviceExitUID = device4.UID,
				LockControlDeviceUID = device5.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Add(door);
			Compile();
		}

		[TestMethod]
		public void TestAntipassback3()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var device3 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var device4 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var device5 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.Barrier,
				AntipassbackOn = true,
				EnterDeviceUID = device1.UID,
				ExitDeviceUID = device2.UID,
				LockDeviceUID = device3.UID,
				//LockControlDevice
				LockDeviceExitUID = device4.UID,
				LockControlDeviceUID = device5.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Add(door);
			Compile();
		}
	}
}