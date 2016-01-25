using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhClient;
using RubezhAPI.GK;
using RubezhAPI;

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
		public void TestDoor1()
		{
			var cardReader1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var cardReader2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var am1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var rm1 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.TwoWay,
				EnterDeviceUID = cardReader1.UID,
				ExitDeviceUID = cardReader2.UID,
				LockDeviceUID = rm1.UID,
				LockControlDeviceUID = am1.UID,
			};
			GKManager.Doors.Add(door);
			Compile();
		}

		[TestMethod]
		public void TestDoor2()
		{
			var cardReader1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var cardReader2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var am1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var rm1 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var rm2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.Turnstile,
				EnterDeviceUID = cardReader1.UID,
				ExitDeviceUID = cardReader2.UID,
				LockDeviceUID = rm1.UID,
				LockDeviceExitUID = rm2.UID,
				LockControlDeviceUID = am1.UID,
			};
			GKManager.Doors.Add(door);
			Compile();
		}

		[TestMethod]
		public void TestDoor3()
		{
			var cardReader1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var cardReader2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var rm1 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var rm2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var am1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var am2 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.Barrier,
				EnterDeviceUID = cardReader1.UID,
				ExitDeviceUID = cardReader2.UID,
				LockDeviceUID = rm1.UID,
				LockDeviceExitUID = rm2.UID,
				LockControlDeviceUID = am1.UID,
				LockControlDeviceExitUID = am2.UID
			};
			GKManager.Doors.Add(door);
			Compile();
		}

		[TestMethod]
		public void TestDoor4()
		{
			var cardReader1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var cardReader2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var am1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var am2 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var am3 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var am4 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var rm1 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var rm2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.AirlockBooth,
				EnterDeviceUID = cardReader1.UID,
				ExitDeviceUID = cardReader2.UID,
				LockDeviceUID = rm1.UID,
				LockDeviceExitUID = rm2.UID,
				LockControlDeviceUID = am1.UID,
				LockControlDeviceExitUID = am2.UID,
				EnterButtonUID = am3.UID,
				ExitButtonUID = am4.UID,
			};
			GKManager.Doors.Add(door);
			Compile();
		}

		[TestMethod]
		public void TestAntipassback1()
		{
			var cardReader1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var cardReader2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var am1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var rm1 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.TwoWay,
				AntipassbackOn = true,
				EnterDeviceUID = cardReader1.UID,
				ExitDeviceUID = cardReader2.UID,
				LockDeviceUID = rm1.UID,
				LockControlDeviceUID = am1.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Add(door);
			Compile();
		}

		[TestMethod]
		public void TestAntipassback2()
		{
			var cardReader1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var cardReader2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var am1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var rm1 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var rm2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.Turnstile,
				AntipassbackOn = true,
				EnterDeviceUID = cardReader1.UID,
				ExitDeviceUID = cardReader2.UID,
				LockDeviceUID = rm1.UID,
				LockDeviceExitUID = rm2.UID,
				LockControlDeviceUID = am1.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Add(door);
			Compile();
		}

		[TestMethod]
		public void TestAntipassback3()
		{
			var cardReader1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var cardReader2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var rm1 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var rm2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var am1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var am2 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.Barrier,
				AntipassbackOn = true,
				EnterDeviceUID = cardReader1.UID,
				ExitDeviceUID = cardReader2.UID,
				LockDeviceUID = rm1.UID,
				LockDeviceExitUID = rm2.UID,
				LockControlDeviceUID = am1.UID,
				LockControlDeviceExitUID = am2.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Add(door);
			Compile();
		}

		[TestMethod]
		public void TestAntipassback4()
		{
			var cardReader1 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var cardReader2 = AddDevice(kauDevice1, GKDriverType.RSR2_CardReader);
			var am1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var am2 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var am3 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var am4 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var rm1 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var rm2 = AddDevice(kauDevice1, GKDriverType.RSR2_RM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor()
			{
				DoorType = GKDoorType.AirlockBooth,
				AntipassbackOn = true,
				EnterDeviceUID = cardReader1.UID,
				ExitDeviceUID = cardReader2.UID,
				LockDeviceUID = rm1.UID,
				LockDeviceExitUID = rm2.UID,
				LockControlDeviceUID = am1.UID,
				LockControlDeviceExitUID = am2.UID,
				EnterButtonUID = am3.UID,
				ExitButtonUID = am4.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Add(door);
			Compile();
		}
	}
}