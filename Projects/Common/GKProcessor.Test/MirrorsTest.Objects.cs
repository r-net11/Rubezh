using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI.GK;
using RubezhClient;

namespace GKProcessor.Test
{
	public partial class MirrorsTest
	{
		[TestMethod]
		public void TestDetectorDevice()
		{
			var device = AddDevice(kauDevice, GKDriverType.RSR2_SmokeDetector);
			Test(device, GKDriverType.DetectorDevicesMirror);
			CheckDeviceHasNoLogic(device);
		}

		[TestMethod]
		public void TestControlDevice()
		{
			var device = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
			Test(device, GKDriverType.ControlDevicesMirror);

			var direction = new GKDirection();
			GKManager.AddDirection(direction);
			Test(direction, GKDriverType.ControlDevicesMirror);

			var delay = new GKDelay();
			GKManager.AddDelay(delay);
			Test(delay, GKDriverType.ControlDevicesMirror);

			var mpt = new GKMPT();
			GKManager.AddMPT(mpt);
			Test(mpt, GKDriverType.ControlDevicesMirror);
			var mptDevice = AddDevice(kauDevice, GKDriverType.RSR2_RM_1);
			mpt.MPTDevices.Add(new GKMPTDevice { Device = mptDevice, DeviceUID = mptDevice.UID, MPTDeviceType = GKMPTDeviceType.Speaker });
			Compile();
			CheckDeviceLogicOnGK(mptDevice);

			var pumpStation = new GKPumpStation();
			GKManager.AddPumpStation(pumpStation);
			Test(pumpStation, GKDriverType.ControlDevicesMirror);

			var nsDevice = AddDevice(kauDevice, GKDriverType.RSR2_Bush_Fire);
			GKManager.ChangePumpDevices(pumpStation, new List<GKDevice> { nsDevice });
			Compile();
			CheckDeviceLogicOnGK(nsDevice);
			Assert.IsNotNull(pumpStation.Pim);
			CheckObjectOnGK(pumpStation.Pim);
		}

		[TestMethod]
		public void TestDirection()
		{
			var direction = new GKDirection();
			GKManager.AddDirection(direction);
			Test(direction, GKDriverType.ControlDevicesMirror);
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
			CheckDeviceLogicOnGK(device);

			var cardReaderDevice = AddDevice(kauDevice, GKDriverType.RSR2_CardReader);
			var guardZoneDevice = new GKGuardZoneDevice();
			guardZoneDevice.Device = cardReaderDevice;
			guardZoneDevice.DeviceUID = cardReaderDevice.UID;
			guardZoneDevice.CodeReaderSettings = new GKCodeReaderSettings();
			guardZoneDevice.CodeReaderSettings.ChangeGuardSettings = new GKCodeReaderSettingsPart();
			var code = new GKCode();
			GKManager.DeviceConfiguration.Codes.Add(code);
			guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeUIDs.Add(code.UID);
			guardZoneDevice.ActionType = GKGuardZoneDeviceActionType.ChangeGuard;
			guardZoneDevice.CodeReaderSettings.ChangeGuardSettings.CodeReaderEnterType = GKCodeReaderEnterType.CodeAndOne;
			GKManager.AddDeviceToGuardZone(cardReaderDevice, guardZone, guardZoneDevice);
			Compile();
			CheckDeviceLogicOnGK(cardReaderDevice);
			CheckObjectOnGK(code);
			Assert.IsNotNull(guardZone.Pim);
			CheckObjectOnGK(guardZone.Pim);
		}

		[TestMethod]
		public void TestFirefightingZone()
		{
			var device = AddDevice(kauDevice, GKDriverType.RSR2_HandDetector);
			var zone = new GKZone();
			GKManager.Zones.Add(zone);
			device.ZoneUIDs.Add(zone.UID);
			var direction = new GKDirection();
			GKManager.AddDirection(direction);
			TestFirefightingZone(zone, direction);
		}
	}
}
