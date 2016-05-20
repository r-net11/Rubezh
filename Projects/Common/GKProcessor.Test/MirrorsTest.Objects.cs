using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKProcessor.Test
{
	public partial class MirrorsTest
	{
		/// <summary>
		// Проверяет извещательное устройство, которое участвует в извещательном отражении, двух отражениях и двух устройствах ПМФ, что:
		// 1. Нет ошибок при перестроении дескриторов
		// 2. Отражение добавляется в выходные дескрипторы
		// 3. Второе отражение добавляется в выходные дескрипторы
		/// </summary>
		[TestMethod]
		public void TestDetectorDevice()
		{
			var device = AddDevice(kauDevice, GKDriverType.RSR2_SmokeDetector);
			Test(device, GKDriverType.DetectorDevicesMirror);
			CheckDeviceHasNoLogic(device);
		}

		/// <summary>
		// Проверяет исполнительное устройство, направление, задержку, МПТ, НС
		// которые участвуют в отражении, двух отражениях и двух устройствах ПМФ, что:
		// 1. Нет ошибок при перестроении дескриторов
		// 2. Отражение добавляется во входные и выходные дескрипторы
		// 3. Второе отражение добавляется во входные и выходные дескрипторы
		// 4. При добавлении какого-либо объекта, который участвует в отражении в логику исполнительного устройства нет ошибок при перестроении дескрипторов
		// 5. Логика исполнительного устройства, которое зависит от какого-либо объекта, который участвует в отражении тоже попадает на ГК
		// 6. Логика сирены МПТ, которое участвует в образе, попадет на ГК
		// 7. Логика насоса НС, которая участвует в образе, попадет на ГК
		// 8. ПИМ НС, которая участвует в образе, попадет на ГК
		/// </summary>
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

		/// <summary>
		// Проверяет направление, которое участвует в отражении НПЗ, двух отражениях и двух устройствах ПМФ, что:
		// 1. Нет ошибок при перестроении дескриторов
		// 2. Отражение добавляется во входные и выходные дескрипторы
		// 3. Второе отражение добавляется во входные и выходные дескрипторы
		// 4. Логика исполнительного устройства, которое зависит от направления, который участвует в отражении тоже попадает на ГК
		/// </summary>
		[TestMethod]
		public void TestDirection()
		{
			var direction = new GKDirection();
			GKManager.AddDirection(direction);
			Test(direction, GKDriverType.DirectionsMirror);
		}

		/// <summary>
		// Проверяет зону, которая участвует в отражении ЗП, двух отражениях и двух устройствах ПМФ, что:
		// 1. Нет ошибок при перестроении дескриторов
		// 2. Отражение добавляется во входные и выходные дескрипторы
		// 3. Второе отражение добавляется во входные и выходные дескрипторы
		// 4. Логика исполнительного устройства, которое зависит от зоны, которая участвует в отражении тоже попадает на ГК
		/// </summary>
		[TestMethod]
		public void TestZone()
		{
			var device = AddDevice(kauDevice, GKDriverType.RSR2_HandDetector);
			var zone = new GKZone();
			GKManager.Zones.Add(zone);
			device.ZoneUIDs.Add(zone.UID);
			Test(zone, GKDriverType.FireZonesMirror);
		}

		/// <summary>
		// Проверяет охранную зону, которая участвует в отражении ЗО, двух отражениях и двух устройствах ПМФ, что:
		// 1. Нет ошибок при перестроении дескриторов
		// 2. Отражение добавляется во входные и выходные дескрипторы
		// 3. Второе отражение добавляется во входные и выходные дескрипторы
		// 4. Логика исполнительного устройства, которое зависит от охранной зоны, которая участвует в отражении тоже попадает на ГК
		// 5. Логика контроллера Виганта, которые участвует в охранной зоне, попадает на ГК
		// 6. Коды, которые участвуют в контроллере Виганта, попадают на ГК
		// 7. Пим охранной зоны попадает на ГК
		/// </summary>
		[TestMethod]
		public void TestGuardZone()
		{
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

			var guardZone = new GKGuardZone();
			GKManager.AddGuardZone(guardZone);
			GKManager.AddDeviceToGuardZone(guardZone, guardZoneDevice);
			Test(guardZone, GKDriverType.GuardZonesMirror);
			CheckObjectOnGK(code);
			Assert.IsNotNull(guardZone.Pim);
			CheckObjectOnGK(guardZone.Pim);
		}

		/// <summary>
		// Проверяет зону и направление, которые участвуют в отражении ЗПЗ, двух отражениях и двух устройствах ПМФ, что:
		// 1. Нет ошибок при перестроении дескриторов
		// 2. Отражение добавляется во входные и выходные дескрипторы зоны и направления
		// 3. Второе отражение добавляется во входные и выходные дескрипторы зоны и направления
		// 4. Логика исполнительного устройства, которое зависит от зоны или направления, которые участвуют в отражении тоже попадает на ГК
		/// </summary>
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
