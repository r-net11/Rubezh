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
		public void TestGuardZone_WithSetGuardDevice()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.SetGuard, DeviceUID = device1.UID });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnKau(guardZone);
		}

		[TestMethod]
		public void TestGuardZone_WithResetGuardDevice()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.ResetGuard, DeviceUID = device1.UID });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnKau(guardZone);
		}

		[TestMethod]
		public void TestGuardZone_WithAlarmDevice()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.SetAlarm, DeviceUID = device1.UID });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnKau(guardZone);
		}

		[TestMethod]
		public void TestGuardZone_WithChangeDevice()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.ChangeGuard, DeviceUID = device1.UID });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnKau(guardZone);

			var pimGKDescriptor = GkDatabase.Descriptors.FirstOrDefault(x => x.GKBase is GKPim);
			var pimKauDescriptor = Kau1Database.Descriptors.FirstOrDefault(x => x.GKBase is GKPim);
			Assert.IsTrue(pimGKDescriptor.Formula.FormulaOperations.Count == 1, "На ГК должна отсутствовать логика ПИМ");
			Assert.IsTrue(pimKauDescriptor.Formula.FormulaOperations.Count > 1, "На КАУ должна присутствовать логика ПИМ");
		}

		/// <summary>
		/// Если в охранную зону входят устройства с одного КАУ, то такая охранная зона попадает на КАУ
		/// </summary>
		[TestMethod]
		public void TestGuardZoneOnKau()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var device2 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.SetGuard, DeviceUID = device1.UID });
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.ResetGuard, DeviceUID = device2.UID });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnKau(guardZone);
		}

		/// <summary>
		/// Если в охранную зону входят устройства с разных КАУ, то охранная зона попадает на ГК
		/// </summary>
		[TestMethod]
		public void TestGuardZoneWithAMOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_AM_1);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.SetGuard, DeviceUID = device1.UID });
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.ResetGuard, DeviceUID = device2.UID });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnGK(guardZone);
		}

		/// <summary>
		/// Если в охранную зону входят устройства с разных КАУ, то охранная зона попадает на ГК
		/// </summary>
		[TestMethod]
		public void TestGuardZoneWithCodeReaderOnGK()
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_AM_1);
			var device2 = AddDevice(kauDevice2, GKDriverType.RSR2_CodeReader);
			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { ActionType = GKGuardZoneDeviceActionType.SetGuard, DeviceUID = device1.UID });
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device2.UID, CodeReaderSettings = new GKCodeReaderSettings() { SetGuardSettings = new GKCodeReaderSettingsPart() { CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne, CodeUIDs = { code.UID } } } });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnGK(guardZone);
		}

		[TestMethod]
		public void TestGuardZoneWithLevelsOnGK()
		{
			TestGuardZoneWithLevelsOnGK(GKDriverType.RSR2_CardReader);
			TestGuardZoneWithLevelsOnGK(GKDriverType.RSR2_CodeReader);
		}

		/// <summary>
		/// Если в охранную зону входит контроллер Wiegand или кодонаборник и указан только уровень и не указан код, то
		/// охранная зона и входящие в нее устройства должны попасть на ГК
		/// </summary>
		public void TestGuardZoneWithLevelsOnGK(GKDriverType driverType)
		{
			var device1 = AddDevice(kauDevice1, driverType);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device1.UID, CodeReaderSettings = new GKCodeReaderSettings() { SetGuardSettings = new GKCodeReaderSettingsPart() { CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne, AccessLevel = 1 } } });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnGK(guardZone);
		}

		[TestMethod]
		public void TestGuardZoneWithCodesOnKau()
		{
			TestGuardZoneWithCodesOnKau(GKDriverType.RSR2_CardReader);
			TestGuardZoneWithCodesOnKau(GKDriverType.RSR2_CodeReader);
		}
		/// <summary>
		/// Если в охранноую зону входит контроллер Wiegand или кодонаборник, а у кодонаборника указан только код и не указан уровень, то такая зона и само устройство
		/// должны попасть на КАУ
		/// </summary>
		void TestGuardZoneWithCodesOnKau(GKDriverType driverType)
		{
			var device1 = AddDevice(kauDevice1, GKDriverType.RSR2_CodeReader);
			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			var guardZone = new GKGuardZone();
			var guardZoneDevice = new GKGuardZoneDevice
			{
				DeviceUID = device1.UID,
				CodeReaderSettings = new GKCodeReaderSettings
				{
					SetGuardSettings = new GKCodeReaderSettingsPart {CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne, CodeUIDs = {code.UID}}
				}
			};
			GKManager.AddDeviceToGuardZone(guardZone, guardZoneDevice);
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnKau(guardZone);
		}

		[TestMethod]
		public void TestGuardZoneWithCodesAndLevelsOnGK()
		{
			TestGuardZoneWithCodesAndLevelsOnGK(GKDriverType.RSR2_CardReader);
			TestGuardZoneWithCodesAndLevelsOnGK(GKDriverType.RSR2_CodeReader);
		}

		/// <summary>
		/// Если в охранной зоне учавствует контроллер Wiegand или кодонаборник и у них настроены и код и уровень доступа, то такая зона должна попасть на ГК
		/// и логика всех устройств, учавствующих в зоне, должна также попасть на ГК
		/// </summary>
		/// <param name="driverType"></param>
		void TestGuardZoneWithCodesAndLevelsOnGK(GKDriverType driverType)
		{
			var device1 = AddDevice(kauDevice1, driverType);
			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device1.UID, CodeReaderSettings = new GKCodeReaderSettings() { SetGuardSettings = new GKCodeReaderSettingsPart() { CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne, CodeUIDs = { code.UID }, AccessLevel = 1 } } });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnGK(guardZone);
		}

		[TestMethod]
		public void TestGuardZoneOnGKAndDeviceLogicOnGK()
		{
			TestGuardZoneOnGKAndDeviceLogicOnGK(GKDriverType.RSR2_CardReader, GKDriverType.RSR2_CardReader);
			TestGuardZoneOnGKAndDeviceLogicOnGK(GKDriverType.RSR2_CardReader, GKDriverType.RSR2_CodeReader);
			TestGuardZoneOnGKAndDeviceLogicOnGK(GKDriverType.RSR2_CodeReader, GKDriverType.RSR2_CardReader);
			TestGuardZoneOnGKAndDeviceLogicOnGK(GKDriverType.RSR2_CodeReader, GKDriverType.RSR2_CodeReader);
		}

		/// <summary>
		/// Если в охранной зоне учавствует контроллер Wiegand или кодонаборник и у них настроен уровень доступа, то такая зона должна попасть на ГК
		/// и логика всех устройств, учавствующих в зоне, должна также попасть на ГК
		/// </summary>
		/// <param name="driverType1"></param>
		/// <param name="driverType2"></param>
		void TestGuardZoneOnGKAndDeviceLogicOnGK(GKDriverType driverType1, GKDriverType driverType2)
		{
			var device1 = AddDevice(kauDevice1, driverType1);
			var device2 = AddDevice(kauDevice1, driverType2);
			var device3 = AddDevice(kauDevice1, GKDriverType.RSR2_GuardDetector);
			var device4 = AddDevice(kauDevice1, GKDriverType.RSR2_GuardDetectorSound);
			var guardZone = new GKGuardZone();
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device1.UID, CodeReaderSettings = new GKCodeReaderSettings() { SetGuardSettings = new GKCodeReaderSettingsPart() { CodeReaderEnterType = GKCodeReaderEnterType.CodeOnly, AccessLevel = 1 } } });
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device2.UID, CodeReaderSettings = new GKCodeReaderSettings() { SetGuardSettings = new GKCodeReaderSettingsPart() { CodeReaderEnterType = GKCodeReaderEnterType.CodeOnly, AccessLevel = 1 } } });
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device3.UID, ActionType = GKGuardZoneDeviceActionType.SetAlarm });
			guardZone.GuardZoneDevices.Add(new GKGuardZoneDevice() { DeviceUID = device4.UID, ActionType = GKGuardZoneDeviceActionType.SetAlarm });
			GKManager.GuardZones.Add(guardZone);
			Compile();

			CheckObjectLogicOnGK(guardZone);
			CheckDeviceLogicOnGK(device3);
			CheckDeviceLogicOnGK(device4);
		}
	}
}