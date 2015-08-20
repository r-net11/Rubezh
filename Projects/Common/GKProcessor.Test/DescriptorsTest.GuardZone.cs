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

		[TestMethod]
		public void TestGuardZoneOnGK()
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

		[TestMethod]
		public void TestGuardZoneWithLevelsOnKau()
		{
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