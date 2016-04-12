using System.Threading;
using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.GK;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GKIntegratedTest
{
	public partial class ItegratedTest
	{

		[Test]
		//RG-1223 (Сброс зоны не должен сбрасывать тревогу, если тревожный датчик в сработке1, тест с контроллером Wiegand)
		public void TestGuardZoneFire1AfterResetCaseWithCardReader()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var cardReader = new GKGuardZoneDevice {Device = device1, DeviceUID = device1.UID};
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM = new GKGuardZoneDevice {Device = device2, DeviceUID = device2.UID};
			aM.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			var device3 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var guardDetector = new GKGuardZoneDevice {Device = device3, DeviceUID = device3.UID};
			guardDetector.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			var zone = new GKGuardZone {Name = "Охранная зона", No = 1};
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, aM);
			GKManager.AddDeviceToGuardZone(zone, cardReader);
			GKManager.AddDeviceToGuardZone(zone, guardDetector);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Off, 10000, "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Off, "Проверка того, что зона снята с охраны");
			ConrtolGKBase(zone, GKStateBit.TurnOn_InAutomatic, "Постановка зоны на охрану");
			WaitWhileState(zone, XStateClass.On, 3000, "Ждем пока зона не встанет на охрану");
			Assert.IsTrue(zone.State.StateClass == XStateClass.On, "Проверка того, что зона установилась на охрану");
			ConrtolGKBase(device3, GKStateBit.Fire1, "Сработка тревоги у датчика");
			WaitWhileState(zone, XStateClass.Fire1, 10000, "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");
			CheckTime(() => Thread.Sleep(1000), "Ждем 1 секунду");
			Assert.IsTrue(device3.State.StateClass == XStateClass.Fire1, "Проверка того, что датчик ещё в тревоге");
			ConrtolGKBase(zone, GKStateBit.Reset, "Cброс зоны");
			WaitWhileState(zone, XStateClass.Fire1, 10000, "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");

		}

		[Test]
		//RG-1223(Сброс зоны не должен сбрасывать тревогу, если тревожный датчик в сработке1, тест без контроллера Wiegand)
		public void TestGuardZoneFire1AfterResetCaseWithoutCardReader()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM = new GKGuardZoneDevice {Device = device1, DeviceUID = device1.UID};
			aM.ActionType = GKGuardZoneDeviceActionType.ChangeGuard;
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var guardDetector = new GKGuardZoneDevice {Device = device2, DeviceUID = device2.UID};
			guardDetector.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			var zone = new GKGuardZone {Name = "Охранная зона", No = 1};
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, aM);
			GKManager.AddDeviceToGuardZone(zone, guardDetector);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Off, 10000, "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Off, "Проверка того, что зона снята с охраны");
			ConrtolGKBase(zone, GKStateBit.TurnOn_InAutomatic, "Постановка зоны на охрану");
			WaitWhileState(zone, XStateClass.On, 3000, "Ждем пока зона не встанет на охрану");
			Assert.IsTrue(zone.State.StateClass == XStateClass.On, "Проверка того, что зона установилась на охрану");
			ConrtolGKBase(device2, GKStateBit.Fire1, "Сработка тревоги у датчика");
			WaitWhileState(zone, XStateClass.Fire1, 10000, "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");
			CheckTime(() => Thread.Sleep(1000), "Ждем 1 секунду");
			Assert.IsTrue(device2.State.StateClass == XStateClass.Fire1, "Проверка того, что датчик ещё в тревоге");
			ConrtolGKBase(zone, GKStateBit.Reset, "Cброс зоны");
			WaitWhileState(zone, XStateClass.Fire1, 10000, "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");
		}

		[Test]
		//RG-1034(Фиксация АМ в сработке в режиме "Изменение" не должно приводить в постоянной выдаче сообщений о постановке и снятия зоны  с охраны)
		public void TestGuardZoneFire1NotChange()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM = new GKGuardZoneDevice {Device = device1, DeviceUID = device1.UID};
			aM.ActionType = GKGuardZoneDeviceActionType.ChangeGuard;
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var guardDetector = new GKGuardZoneDevice { Device = device2, DeviceUID = device2.UID };
			guardDetector.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			var zone = new GKGuardZone {Name = "Охранная зона", No = 1};
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, aM);
			GKManager.AddDeviceToGuardZone(zone, guardDetector);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Off, 10000, "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Off, "Проверка того, что зона снята с охраны");
			ConrtolGKBase(zone, GKStateBit.TurnOn_InAutomatic, "Постановка зоны на охрану");
			WaitWhileState(zone, XStateClass.On, 3000, "Ждем пока зона не встанет на охрану");
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка тревоги у АМ");
			ConrtolGKBase(device2, GKStateBit.Fire1, "Сработка тревоги у датчика");
			WaitWhileState(zone, XStateClass.Fire1, 10000, "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(device1.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что АМ перешёл в сработку");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");
			for (int i = 0; i < 30; i++)
			{
				CheckTime(() => Thread.Sleep(150), "Ждём 150 милисекунд");
				Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверяем что зона в тревоге");
			}

		}
	}
}