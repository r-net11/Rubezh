using System;
using System.Collections.Generic;
using System.Linq;
using GKModule.Validation;
using Infrastructure.Common.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI.GK;
using RubezhClient;
using RubezhAPI;

namespace GKProcessor.Test
{
	public partial class ValidationTest
	{
		[TestMethod]
		public void TestOneWayHasNoDevices()
		{
			var enterDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var exitDevice = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var lockDevice = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var lockControlDevice = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var door = new GKDoor
			{
				DoorType = GKDoorType.OneWay,
				EnterDeviceUID = enterDevice.UID,
				ExitDeviceUID = exitDevice.UID,
				LockDeviceUID = lockDevice.UID,
				LockControlDeviceUID = lockControlDevice.UID,
			};
			GKManager.Doors.Clear();
			GKManager.Doors.Add(door);
			var errors = Validate();
			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));
			door.EnterDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));
			door.ExitDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключен замок"));
			door.LockDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключен замок"));
		}

		[TestMethod]
		public void TestTwoWayHasNoDevices()
		{
			var enterDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var exitDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CodeReader);
			var lockDevice = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var lockControlDevice = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor
			{
				DoorType = GKDoorType.TwoWay,
				EnterDeviceUID = enterDevice.UID,
				ExitDeviceUID = exitDevice.UID,
				LockDeviceUID = lockDevice.UID,
				LockControlDeviceUID = lockControlDevice.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Clear();
			GKManager.Doors.Add(door);
			var errors = Validate();
			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));
			door.EnterDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));
			door.ExitDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключен замок"));
			door.LockDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключен замок"));

			door.AntipassbackOn = true;
			errors = Validate();

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик контроля двери"));
			door.LockControlDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик контроля двери"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на вход"));
			door.EnterZoneUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на выход"));
			door.ExitZoneUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на выход"));
		}

		[TestMethod]
		public void TestTurnstileHasNoDevices()
		{
			var enterDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var exitDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CodeReader);
			var lockDevice = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var lockDeviceExit = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var lockControlDevice = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor
			{
				DoorType = GKDoorType.Turnstile,
				EnterDeviceUID = enterDevice.UID,
				ExitDeviceUID = exitDevice.UID,
				LockDeviceUID = lockDevice.UID,
				LockDeviceExitUID = lockDeviceExit.UID,
				LockControlDeviceUID = lockControlDevice.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Clear();
			GKManager.Doors.Add(door);
			var errors = Validate();
			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));
			door.EnterDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));
			door.ExitDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на вход"));
			door.LockDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на выход"));
			door.LockDeviceExitUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на выход"));

			door.AntipassbackOn = true;
			errors = Validate();

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик проворота"));
			door.LockControlDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик проворота"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на вход"));
			door.EnterZoneUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на выход"));
			door.ExitZoneUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на выход"));
		}

		[TestMethod]
		public void TestBarrierHasNoDevices()
		{
			var enterDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var exitDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CodeReader);
			var lockDevice = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var lockDeviceExit = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var lockControlDevice = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var lockControlDeviceExit = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor
			{
				DoorType = GKDoorType.Barrier,
				EnterDeviceUID = enterDevice.UID,
				ExitDeviceUID = exitDevice.UID,
				LockDeviceUID = lockDevice.UID,
				LockDeviceExitUID = lockDeviceExit.UID,
				LockControlDeviceUID = lockControlDevice.UID,
				LockControlDeviceExitUID = lockControlDeviceExit.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Clear();
			GKManager.Doors.Add(door);
			var errors = Validate();
			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));
			door.EnterDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));
			door.ExitDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на открытие"));
			door.LockDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на открытие"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на закрытие"));
			door.LockDeviceExitUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на закрытие"));

			door.AntipassbackOn = true;
			errors = Validate();

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на вход"));
			door.EnterZoneUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на выход"));
			door.ExitZoneUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на выход"));
		}

		[TestMethod]
		public void TestAirlockBoothHasNoDevices()
		{
			var enterDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var exitDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CodeReader);
			var lockDevice = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var lockDeviceExit = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var lockControlDevice = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var lockControlDeviceExit = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var enterButton = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var exitButton = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var zone1 = new GKSKDZone();
			GKManager.SKDZones.Add(zone1);
			var zone2 = new GKSKDZone();
			GKManager.SKDZones.Add(zone2);
			var door = new GKDoor
			{
				DoorType = GKDoorType.AirlockBooth,
				EnterDeviceUID = enterDevice.UID,
				ExitDeviceUID = exitDevice.UID,
				LockDeviceUID = lockDevice.UID,
				LockDeviceExitUID = lockDeviceExit.UID,
				LockControlDeviceUID = lockControlDevice.UID,
				LockControlDeviceExitUID = lockControlDeviceExit.UID,
				EnterButtonUID = enterButton.UID,
				ExitButtonUID = exitButton.UID,
				EnterZoneUID = zone1.UID,
				ExitZoneUID = zone2.UID
			};
			GKManager.Doors.Clear();
			GKManager.Doors.Add(door);
			var errors = Validate();
			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));
			door.EnterDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));
			door.ExitDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на вход"));
			door.LockDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на выход"));
			door.LockDeviceExitUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на выход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключена кнопка на вход"));
			door.EnterButtonUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключена кнопка на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключена кнопка на выход"));
			door.ExitButtonUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключена кнопка на выход"));

			door.AntipassbackOn = true;
			errors = Validate();

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик контроля двери"));
			door.LockControlDeviceUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик контроля двери"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик контроля двери на выход"));
			door.LockControlDeviceExitUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик контроля двери на выход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на вход"));
			door.EnterZoneUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на вход"));

			Assert.IsFalse(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на выход"));
			door.ExitZoneUID = Guid.Empty;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на выход"));
		}

		[TestMethod]
		public void TestDoorHasSameEnterAndExitDevices()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var door = new GKDoor
			{
				DoorType = GKDoorType.AirlockBooth,
				EnterDeviceUID = device.UID,
				ExitDeviceUID = device.UID
			};
			GKManager.Doors.Clear();
			GKManager.Doors.Add(door);

			var errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Устройство " + device.PresentationName + " не может быть одновременно устройством на вход и устройством на выход"));
		}

		[TestMethod]
		public void TestDoorHasSameEnterAndExitLockDevices()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var door = new GKDoor
			{
				DoorType = GKDoorType.AirlockBooth,
				LockDeviceUID = device.UID,
				LockDeviceExitUID = device.UID
			};
			GKManager.Doors.Clear();
			GKManager.Doors.Add(door);

			var errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Устройство " + device.PresentationName + " не может быть одновременно реле на вход и реле на выход"));
			door.DoorType = GKDoorType.Barrier;
			errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Устройство " + device.PresentationName + " не может быть одновременно реле на открытие и реле на закрытие"));
		}

		[TestMethod]
		public void TestDoorHasSameEnterAndExitButtonDevices()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var door = new GKDoor
			{
				DoorType = GKDoorType.AirlockBooth,
				EnterButtonUID = device.UID,
				ExitButtonUID = device.UID
			};
			GKManager.Doors.Clear();
			GKManager.Doors.Add(door);

			var errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Устройство " + device.PresentationName + " уже участвует в точке доступа"));
		}

		[TestMethod]
		public void TestDoorsHaveSameDevices()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var door1 = new GKDoor
			{
				DoorType = GKDoorType.AirlockBooth,
				EnterDeviceUID = device.UID,
			};
			GKManager.Doors.Clear();
			GKManager.Doors.Add(door1);

			var door2 = new GKDoor
			{
				DoorType = GKDoorType.AirlockBooth,
				EnterDeviceUID = device.UID,
			};
			GKManager.Doors.Add(door2);

			var errors = Validate();
			Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Устройство " + device.PresentationName + " уже участвует в другой точке доступа"));
		}

		[TestMethod]
		public void TestDoorHasNoDevices()
		{
			foreach (var doorType in new List<GKDoorType>(Enum.GetValues(typeof(GKDoorType)).Cast<GKDoorType>()))
			{
				var door = new GKDoor {DoorType = doorType};
				GKManager.Doors.Clear();
				GKManager.Doors.Add(door);
				var errors = Validate();
				if (doorType == GKDoorType.Barrier)
				{ 
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Для шлагбаума должен быть задан датчик контроля на въезд"));
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "Для шлагбаума должен быть задан датчик контроля на выезд"));
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на открытие"));
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на закрытие"));
				}
				else if (door.DoorType == GKDoorType.AirlockBooth || door.DoorType == GKDoorType.Turnstile)
				{
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на вход"));
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено реле на выход"));
					if (door.DoorType == GKDoorType.AirlockBooth)
					{
						Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключена кнопка на вход"));
						Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключена кнопка на выход"));
					}
				}
				else
				{
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключен замок"));
				}

				Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на вход"));
				Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "К точке доступа не подключено устройство на выход"));

				door.AntipassbackOn = true;
				errors = Validate();
				if (door.DoorType == GKDoorType.OneWay)
					return;
				if (door.DoorType == GKDoorType.Turnstile)
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик проворота"));
				else
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик контроля двери"));
				if (door.DoorType == GKDoorType.AirlockBooth)
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует датчик контроля двери на выход"));
				if (door.EnterZoneUID == Guid.Empty)
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на вход"));
				if (door.DoorType != GKDoorType.OneWay && door.ExitZoneUID == Guid.Empty)
					Assert.IsTrue(errors.Any(x => x.ErrorLevel == ValidationErrorLevel.CannotWrite && x.Error == "При включенном Antipassback, отсутствует зона на выход"));
			}
		}
	}
}
