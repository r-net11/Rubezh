using System.Threading;
using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
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
			Assert.IsTrue(device3.State.StateClass == XStateClass.Off, "Проверка того, что датчик выключен");
			ConrtolGKBase(zone, GKStateBit.TurnOn_InAutomatic, "Постановка зоны на охрану");
			WaitWhileState(zone, XStateClass.On, 3000, "Ждем пока зона не встанет на охрану");
			Assert.IsTrue(zone.State.StateClass == XStateClass.On, "Проверка того, что зона установилась на охрану");
			ConrtolGKBase(device3, GKStateBit.Fire1, "Сработка тревоги у датчика");
			WaitWhileState(zone, XStateClass.Fire1, 10000, "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");
			CheckTime(() => Thread.Sleep(1000), "Ждем 1 секунду");
			Assert.IsTrue(device3.State.StateClass == XStateClass.Fire1, "Проверка того, что датчик ещё в тревоге");
			CheckJournal(5, JournalItem(zone, JournalEventNameType.На_охране),
				JournalItem(device3, JournalEventNameType.Включено), JournalItem(device3, JournalEventNameType.Сработка_1),
				JournalItem(zone, JournalEventNameType.Внимание), JournalItem(zone, JournalEventNameType.Тревога));
			ConrtolGKBase(zone, GKStateBit.Reset, "Cброс зоны");
			WaitWhileOneOfStates(zone, XStateClass.Fire1, 10000, "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");
			Thread.Sleep(6000);
			CheckJournal(3, JournalItem(zone, JournalEventNameType.Норма),
				JournalItem(zone, JournalEventNameType.Внимание), JournalItem(zone, JournalEventNameType.Тревога));

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
			CheckJournal(5, JournalItem(zone, JournalEventNameType.На_охране),
				JournalItem(device2, JournalEventNameType.Включено), JournalItem(device2, JournalEventNameType.Сработка_1),
				JournalItem(zone, JournalEventNameType.Внимание), JournalItem(zone, JournalEventNameType.Тревога));
			ConrtolGKBase(zone, GKStateBit.Reset, "Cброс зоны");
			ConrtolGKBase(zone, GKStateBit.Reset, "Cброс зоны");
			WaitWhileState(zone, XStateClass.Fire1, 10000, "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");
			Thread.Sleep(6000);
			CheckJournal(3, JournalItem(zone, JournalEventNameType.Норма),
				JournalItem(zone, JournalEventNameType.Внимание), JournalItem(zone, JournalEventNameType.Тревога));
		}

		[TestCase(XStateClass.Off, XStateClass.On, JournalEventNameType.Не_на_охране, JournalEventNameType.На_охране)]
		[TestCase(XStateClass.On, XStateClass.Off, JournalEventNameType.На_охране, JournalEventNameType.Не_на_охране)]
		/* RG-1034(Фиксация АМ в сработке в режиме "Изменение" не должно приводить к
		 постоянной выдаче сообщений о постановке и снятия зоны  с охраны) */
		public void TestGuardZoneFire1NotChange(XStateClass mode1, XStateClass mode2, JournalEventNameType event1,
			JournalEventNameType event2)
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
			{
				ConrtolGKBase(zone, GKStateBit.TurnOn_InAutomatic, "включаем зону, case#1");
				WaitWhileState(zone, XStateClass.On, 4000, "Ждём пока зона не встанет на охрану");
				CheckJournal(1, JournalItem(zone, JournalEventNameType.На_охране));
			}
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка1 у АМ");
			WaitWhileState(device1, XStateClass.Fire1, 10000, "Ждем пока АМ не перейдёт в режим сработка1");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Fire1, "Проверка того, что АМ перешёл в сработку");
			WaitWhileState(zone, mode1, 3000, "Ждем пока зона не установится в mode1");
			Assert.IsTrue(zone.State.StateClasses.Contains(mode1), "Проверка того, что зона установилась в mode1");
			WaitWhileState(zone, mode2, 2000, "Ждём 6 секунды, зона не должна перейти в mode2 ");
			Assert.IsFalse(zone.State.StateClasses.Contains(mode2), "Проверка того, что зона не перешла в mode2");
			ConrtolGKBase(device1, GKStateBit.Reset, "Сбрасываем АМ");
			WaitWhileState(device1, XStateClass.Norm, 4000, "Ждем пока АМ не выключится");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Norm, "Проверка того, что АМ выключен");
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка1 у АМ");
			WaitWhileState(device1, XStateClass.Fire1, 10000, "Ждем пока АМ не перейдёт в режим сработка1");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Fire1, "Проверка того, что АМ перешёл в сработку");
			WaitWhileOneOfStates(zone, mode2, 4000, "Ждем пока зона не установится в mode2");
			Assert.IsTrue(zone.State.StateClasses.Contains(mode2), "Проверка того, что зона установилась в mode2");
			WaitWhileState(zone, mode1, 2000, "Ждём 2 секунды, зона не должна перейти в mode1 ");
			Assert.IsFalse(zone.State.StateClasses.Contains(mode1), "Проверка того, что зона не перешла в mode1");
			CheckJournal(5, JournalItem(device1, JournalEventNameType.Сработка_1),
				JournalItem(zone, event1), JournalItem(device1, JournalEventNameType.Норма),
				JournalItem(device1, JournalEventNameType.Сработка_1), JournalItem(zone, event2));
		}

		[Test]
		/* RG-1030 (Не работает невзятие если выполнить сработку 2 для АМ, и впоследствии взять зону на охрану)*/
		public void TestGuardZoneNoAlarmAfterFire2()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM1 = AddGuardZoneDevice(device1);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM2 = AddGuardZoneDevice(device2);
			aM1.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			aM2.ActionType = GKGuardZoneDeviceActionType.SetGuard;
			var zone = new GKGuardZone {Name = "Охранная зона", No = 1};
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, aM1);
			GKManager.AddDeviceToGuardZone(zone, aM2);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Off, 10000, "Ждем норму в охранной зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Off, "Проверка того, что зона снята с охраны");
			ConrtolGKBase(device1, GKStateBit.Fire2, "Сработка2 у тревожного датчика");
			WaitWhileState(device1, XStateClass.Fire2, 6000, "Ждем пока датчик не перейдёт в Сработка2");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Off, "Проверка того, что зона не установилась на охрану");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Fire2, "Проверка того, АМ перешла в сработку 2");
			ConrtolGKBase(device2, GKStateBit.Fire1, "Постановка зоны на охрану");
			WaitWhileState(device2, XStateClass.Fire1, 6000, "Ждем пока АМ не перейдёт в Сработка2");
			Assert.IsTrue(device2.State.StateClass == XStateClass.Fire1, "Проверка того, АМ перешла в сработку 2");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Off, "Проверка невзятия зоны");
			CheckJournal(2, JournalItem(device1, JournalEventNameType.Сработка_2),
				JournalItem(device2, JournalEventNameType.Сработка_1));
		}

		[Test]
		/* RG-1028 (После сброса тревоги в охранной зоне тревога должна включиться вновь, если тревожный датчик остаётся в сработке)*/
		public void TestGuardZoneAlarmAfterResetWhileFire1()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM1 = AddGuardZoneDevice(device1);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM2 = AddGuardZoneDevice(device2);
			aM1.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			aM2.ActionType = GKGuardZoneDeviceActionType.SetGuard;
			var zone = new GKGuardZone {Name = "Охранная зона", No = 1};
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, aM1);
			GKManager.AddDeviceToGuardZone(zone, aM2);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Off, 10000, "Ждем норму в охранной зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Off, "Проверка того, что зона снята с охраны");
			ConrtolGKBase(device2, GKStateBit.Fire1, "Постановка зоны на охрану");
			WaitWhileState(zone, XStateClass.On, 6000, "Ждем пока зона не перейдёт на охрану");
			Assert.IsTrue(zone.State.StateClass == XStateClass.On, "Проверка того, зона установилась на охрану");
			ConrtolGKBase(device1, GKStateBit.Fire1, "Перевод тревожного датчика в тревогу");
			WaitWhileState(zone, XStateClass.Fire1, 6000, "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire1, "Проверка того, зона перешла в тревогу");
			ConrtolGKBase(zone, GKStateBit.Reset, "Сброс тревоги у зоны");
			WaitWhileState(zone, XStateClass.Fire1, 6000, "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire1, "Проверка того, зона в тревоге");
		}
	}
}