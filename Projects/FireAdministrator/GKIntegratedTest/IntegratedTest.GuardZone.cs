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
		/* RG-1223 (Сброс зоны не должен сбрасывать тревогу, если тревожный датчик в сработке1, тест с контроллером Wiegand)*/
		public void TestGuardZoneFire1AfterResetCaseWithCardReader()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var cardReader = AddGuardZoneDevice(device1);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM = AddGuardZoneDevice(device2);
			aM.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			var device3 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var guardDetector = AddGuardZoneDevice(device3);
			guardDetector.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			var zone = new GKGuardZone {Name = "Охранная зона", No = 1};
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, aM);
			GKManager.AddDeviceToGuardZone(zone, cardReader);
			GKManager.AddDeviceToGuardZone(zone, guardDetector);
			SetConfigAndRestartImitator();
			WaitWhileState(zone, XStateClass.Off, 10000, "Ждем норму в охранной зоне");
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
		/*RG-1223(Сброс зоны не должен сбрасывать тревогу, если тревожный датчик в сработке1, тест без контроллера Wiegand) */
		public void TestGuardZoneFire1AfterResetCaseWithoutCardReader()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM = AddGuardZoneDevice(device1);
			aM.ActionType = GKGuardZoneDeviceActionType.ChangeGuard;
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var guardDetector = AddGuardZoneDevice(device2);
			guardDetector.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			var zone = new GKGuardZone {Name = "Охранная зона", No = 1};
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, aM);
			GKManager.AddDeviceToGuardZone(zone, guardDetector);
			SetConfigAndRestartImitator();
			WaitWhileState(zone, XStateClass.Off, 10000, "Ждем норму в охранной зоне");
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

		[TestCase(XStateClass.Off, XStateClass.On)]
		[TestCase(XStateClass.On,XStateClass.Off)]
		/* RG-1034(Фиксация АМ в сработке в режиме "Изменение" не должно приводить к
		 постоянной выдаче сообщений о постановке и снятия зоны  с охраны) */
		public void TestGuardZoneFire1NotChange(XStateClass mode1, XStateClass mode2)
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM = AddGuardZoneDevice(device1);
			aM.ActionType = GKGuardZoneDeviceActionType.ChangeGuard;
			var zone = new GKGuardZone {Name = "Охранная зона", No = 1};
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, aM);
			SetConfigAndRestartImitator();
			WaitWhileState(zone, XStateClass.Off, 10000, "Ждем норму в охранной зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Off, "Проверка того, что зона снята с охраны");
			if (mode1 == XStateClass.Off)
				ConrtolGKBase(zone, GKStateBit.TurnOn_InAutomatic, "включаем зону, case#1");
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка1 у АМ");
			WaitWhileState(device1, XStateClass.Fire1, 10000, "Ждем пока АМ не перейдёт в режим сработка1");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Fire1, "Проверка того, что АМ перешёл в сработку");
			WaitWhileState(zone, mode1, 3000, "Ждем пока зона не установится в mode1");
			Assert.IsTrue(zone.State.StateClasses.Contains(mode1), "Проверка того, что зона установилась в mode1");
			WaitWhileState(zone, mode2, 2000, "Ждём 6 секунды, зона не должна перейти в mode2 ");
			Assert.IsFalse(zone.State.StateClasses.Contains(mode2), "Проверка того, что зона не перешла в mode2");
			ConrtolGKBase(device1, GKStateBit.Reset,"Сбрасываем АМ");
			WaitWhileState(device1, XStateClass.Off, 3000, "Ждем пока АМ не выключится");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Norm, "Проверка того, что АМ выключен");
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка1 у АМ");
			WaitWhileState(device1, XStateClass.Fire1, 10000, "Ждем пока АМ не перейдёт в режим сработка1");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Fire1, "Проверка того, что АМ перешёл в сработку");
			WaitWhileState(zone, mode2, 3000, "Ждем пока зона не установится в mode2");
			Assert.IsTrue(zone.State.StateClasses.Contains(mode2), "Проверка того, что зона установилась в mode2");
			WaitWhileState(zone, mode1, 2000, "Ждём 2 секунды, зона не должна перейти в mode1 ");
			Assert.IsFalse(zone.State.StateClasses.Contains(mode1), "Проверка того, что зона не перешла в mode1");
		}
	}
}