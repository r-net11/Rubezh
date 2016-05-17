using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GKIntegratedTest
{
	public partial class ItegratedTest
	{
		[TestCase(GKDriverType.RSR2_AM_1)]
		[TestCase(GKDriverType.RSR2_SmokeDetector)]
		[TestCase(GKDriverType.RSR2_MAP4)]
		[TestCase(GKDriverType.RSR2_HeatDetector)]
		[TestCase(GKDriverType.RSR2_HeatDetectorEridan)]
		[TestCase(GKDriverType.RSR2_CombinedDetector)]
		[TestCase(GKDriverType.RSR2_ABShS)]
		[TestCase(GKDriverType.RSR2_ABTK)]
		[TestCase(GKDriverType.RSR2_IOLIT)]
		//[TestCase(GKDriverType.RSR2_Button)] устройство не поддерживается
		/* Тестирование жизненного цикла датчиков и пожарной зоны в режиме Сработка1 */
		public void TestDeviceFire1(GKDriverType driver)
		{
			var device1 = AddDevice(kauDevice11, driver);
			var device2 = AddDevice(kauDevice11, driver);
			var device3 = AddDevice(kauDevice11, driver);
			var zone = new GKZone {Name = "Пожарная зона", No = 1};
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device1, zone);
			GKManager.AddDeviceToZone(device2, zone);
			GKManager.AddDeviceToZone(device3, zone);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Norm, 10000, "Ждём Норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона находится в Норме");
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка1 в device");
			WaitWhileState(device1, XStateClass.Fire1, 3000, "Ожидаем пока device1 перейдёт в Сработка1");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Fire1, "Проверка того, что device1 находится в Сработка1");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Attention, "Проверка того, что пожараная зона перешла во Внимание");
			CheckJournal(3, JournalItem(device1, JournalEventNameType.Сработка_1),
				JournalItem(zone, JournalEventNameType.Внимание), JournalItem(Led("Устройство Внимание "), JournalEventNameType.Включено));
			ConrtolGKBase(device2, GKStateBit.Fire1, "Сработка1 в device2");
			WaitWhileState(device2, XStateClass.Fire1, 3000, "Ожидаем пока device2 перейдёт в Сработка1");
			Assert.IsTrue(device2.State.StateClass == XStateClass.Fire1, "Проверка того, что device2 находится в Сработка1");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire1, "Проверка того, что пожараная зона перешла в Пожар1");
			CheckJournal(4, JournalItem(device2, JournalEventNameType.Сработка_1),
				JournalItem(Led("Устройство Внимание "), JournalEventNameType.Выключено),
				JournalItem(zone, JournalEventNameType.Пожар_1), JournalItem(Led("Устройство Пожар 1 "), JournalEventNameType.Включено));
			ConrtolGKBase(device1, GKStateBit.Reset, "Сброс device1");
			ConrtolGKBase(zone, GKStateBit.Reset, "Сброс зоны");
			WaitWhileState(device1, XStateClass.Norm, 3000, "Ожидаем пока device1 перейдёт в Норму");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Norm, "Проверка того, что device1 перешёл в Норму");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Attention), "Проверка того, что зона перешла во Внимание");
			/*CheckJournal(5, JournalItem(device1, JournalEventNameType.Норма), JournalItem(zone, JournalEventNameType.Норма),
				JournalItem(Led("Устройство Пожар 1 "), JournalEventNameType.Выключено), JournalItem(zone, JournalEventNameType.Внимание),
				JournalItem(Led("Устройство Внимание "), JournalEventNameType.Включено)); RG-1340*/
			ConrtolGKBase(device2, GKStateBit.Reset, "Сброс device2");
			WaitWhileState(device2, XStateClass.Norm, 3000, "Ожидаем пока device2 перейдёт в Норму");
			Assert.IsTrue(device2.State.StateClass == XStateClass.Norm, "Проверка того, что device2 перешёл в Норму");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона перешла в Норму");
			/*CheckJournal(3, JournalItem(device2, JournalEventNameType.Норма),
				JournalItem(zone, JournalEventNameType.Норма), JournalItem(Led("Устройство Внимание "), JournalEventNameType.Выключено)); RG-1340*/
		}

		[TestCase(GKDriverType.RSR2_HandDetector)]
		[TestCase(GKDriverType.RSR2_HandDetectorEridan)]
		[TestCase(GKDriverType.RSR2_AM_1)]
		[TestCase(GKDriverType.RSR2_MAP4)]
		[TestCase(GKDriverType.RSR2_ABShS)]
		/* Тестирование жизненного цикла датчиков и пожарной зоны в режиме Сработка2 */
		public void TestDeviceFire2(GKDriverType driver)
		{
			var device1 = AddDevice(kauDevice11, driver);
			var device2 = AddDevice(kauDevice11, driver);
			var device3 = AddDevice(kauDevice11, driver);
			var zone = new GKZone {Name = "Пожарная зона", No = 1};
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device1, zone);
			GKManager.AddDeviceToZone(device2, zone);
			GKManager.AddDeviceToZone(device3, zone);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Norm, 10000, "Ждём Норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона находится в Норме");
			ConrtolGKBase(device1, GKStateBit.Fire2, "Сработка2 в device");
			WaitWhileState(device1, XStateClass.Fire2, 3000, "Ожидаем пока device1 перейдёт в Сработка2");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Fire2, "Проверка того, что device1 находится в Сработка2");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire2, "Проверка того, что пожараная зона перешла в Пожар2");
			CheckJournal(3, JournalItem(device1, JournalEventNameType.Сработка_2),
				JournalItem(zone, JournalEventNameType.Пожар_2), JournalItem(Led("Устройство Пожар 2 "), JournalEventNameType.Включено));
			ConrtolGKBase(zone, GKStateBit.Reset, "Сброс зоны");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire2, "Проверка того, что зона всё ещё в режиме Сработка2");
			ConrtolGKBase(device1, GKStateBit.Reset, "Сброс устройства1");
			WaitWhileState(device1, XStateClass.Norm, 3000, "Ожидаем пока device1 перейдёт в Норму");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire2, "Проверка того, что зона всё ещё в режиме Сработка2");
			CheckJournal(2, JournalItem(device1, JournalEventNameType.Норма));
			ConrtolGKBase(zone, GKStateBit.Reset, "Сброс зоны");
			WaitWhileState(zone, XStateClass.Norm, 3000, "Ожидаем пока зона перейдёт в Норму");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона перешла в Норму");
			//CheckJournal(2, JournalItem(zone, JournalEventNameType.Норма), JournalItem(Led("Устройство Пожар 2"), JournalEventNameType.Выключено)); RG-1340 & RG-1341
		}

		[TestCase(DelayRegime.On, XStateClass.On)]
		[TestCase(DelayRegime.Off, XStateClass.Off)]
		/* RG-1015 (Если у направления задан Режим после удержания "Включено", то после окончания отсчета удержания оно не должено переходить в режим "Включается")*/
		public void TestDelayRegimeDirection(DelayRegime regime, XStateClass state)
		{
			var direction = new GKDirection { Name = "Направление", No = 1 };

			var zone = new GKZone { Name = "Пожарная зона", No = 1 };
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllZones,
				StateType = GKStateBit.Fire1,
				ZoneUIDs = { zone.UID }
			};
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_HeatDetector);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_HeatDetector);
			var device3 = AddDevice(kauDevice11, GKDriverType.RSR2_HeatDetector);
			direction.Logic.OnClausesGroup.Clauses.Add(clause);
			direction.Delay = 3;
			direction.Hold = 3;
			direction.DelayRegime = regime;
			GKManager.AddZone(zone);
			GKManager.AddDirection(direction);
			GKManager.AddDeviceToZone(device1, zone);
			GKManager.AddDeviceToZone(device2, zone);
			GKManager.AddDeviceToZone(device3, zone);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Norm, 10000, "Ждем Норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона в Норме");
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка1 у датчика1");
			ConrtolGKBase(device2, GKStateBit.Fire1, "Сработка1 у датчика2");
			WaitWhileState(direction, XStateClass.On, 5000, "Ждем пока направление не Включится");
			Assert.IsTrue(direction.State.StateClass == XStateClass.On, "Проверка того, что направление Включено");
			WaitWhileState(direction, XStateClass.TurningOn, 6000, "Ждем 6 секунд, направление не должено перейти в режим Включается");
			Assert.IsFalse(direction.State.StateClass == XStateClass.TurningOn, "Проверка того, что направление не перешло в режим Включается");
			Assert.IsTrue(direction.State.StateClass == state, "Проверка того, что направление Включено/Выключено");
/*			if (direction.DelayRegime == DelayRegime.On)
				CheckJournal(4, JournalItem(direction, JournalEventNameType.Включается),
							JournalItem(Led("Устройство Включение ПУСК "), JournalEventNameType.Включено), JournalItem(Led("Устройство Пожар 1 "), JournalEventNameType.Включено),
							JournalItem(direction, JournalEventNameType.Включено)); 
			else
				CheckJournal(6, JournalItem(direction, JournalEventNameType.Включается),
					JournalItem(Led("Устройство Включение ПУСК "), JournalEventNameType.Включено), JournalItem(Led("Устройство Пожар 1 "), JournalEventNameType.Включено),
					JournalItem(direction, JournalEventNameType.Включено), JournalItem(direction, JournalEventNameType.Выключено), JournalItem(Led("Устройство Пожар 1 "), JournalEventNameType.Выключено));
RG-1340*/
		}

		[TestCase(DelayRegime.On, XStateClass.On)]
		[TestCase(DelayRegime.Off, XStateClass.Off)]
		/* RG-1015 (Если у задержки задан Режим после удержания "Включено", то после окончания отсчета удержания она не должена переходить в режим "Включается")*/
		public void TestDelayRegimeDelay(DelayRegime regime, XStateClass state)
		{
			var delay = new GKDelay { Name = "Задержка", No = 1 };
			var zone = new GKZone { Name = "Пожарная зона", No = 1 };
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllZones,
				StateType = GKStateBit.Fire1,
				ZoneUIDs = { zone.UID }
			};
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_HeatDetector);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_HeatDetector);
			var device3 = AddDevice(kauDevice11, GKDriverType.RSR2_HeatDetector);
			delay.Logic.OnClausesGroup.Clauses.Add(clause);
			delay.DelayTime = 3;
			delay.Hold = 3;
			delay.DelayRegime = regime;
			GKManager.AddZone(zone);
			GKManager.AddDelay(delay);
			GKManager.AddDeviceToZone(device1, zone);
			GKManager.AddDeviceToZone(device2, zone);
			GKManager.AddDeviceToZone(device3, zone);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Norm, 10000, "Ждем Норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона в Норме");
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка1 у датчика1");
			ConrtolGKBase(device2, GKStateBit.Fire1, "Сработка1 у датчика2");
			WaitWhileState(delay, XStateClass.On, 5000, "Ждем пока задрежка не Включится");
			Assert.IsTrue(delay.State.StateClass == XStateClass.On, "Проверка того, что задержка Включена");
			WaitWhileState(delay, XStateClass.TurningOn, 6000, "Ждем 6 секунд, задрежка не должена перейти в режим Включается");
			Assert.IsFalse(delay.State.StateClass == XStateClass.TurningOn, "Проверка того, что задрежка не перешла в режим Включается");
			Assert.IsTrue(delay.State.StateClass == state, "Проверка того, что задрежка Включена/Выключена");
			if (delay.DelayRegime == DelayRegime.On)
				CheckJournal(3, JournalItem(delay, JournalEventNameType.Включается), JournalItem(Led("Устройство Пожар 1 "), JournalEventNameType.Включено), JournalItem(delay, JournalEventNameType.Включено));
			else
				CheckJournal(4, JournalItem(delay, JournalEventNameType.Включается), JournalItem(Led("Устройство Пожар 1 "), JournalEventNameType.Включено), JournalItem(delay, JournalEventNameType.Включено), JournalItem(delay, JournalEventNameType.Выключено));
		}
	}
}
