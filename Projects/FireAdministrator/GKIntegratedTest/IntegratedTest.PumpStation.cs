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
		/* RG-1004 (Включение НС по условию - пожар1 в зоне)*/
		public void TestPumpStaitionStartLogicWithFirezone()
		{
			var pumpStaition = new GKPumpStation {Name = "Насосная станция", No = 1};
			var zone = new GKZone {Name = "Пожарная зона", No = 1};
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllZones,
				StateType = GKStateBit.Fire1,
				ZoneUIDs = {zone.UID}
			};
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_HeatDetector);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_HeatDetector);
			var device3 = AddDevice(kauDevice11, GKDriverType.RSR2_HeatDetector);
			var pump = AddDevice(kauDevice11, GKDriverType.RSR2_Bush_Fire);
			var gkpim = AddPim(gkDevice1);
			var kaupim = AddPim(kauDevice11);
			pumpStaition.StartLogic.OnClausesGroup.Clauses.Add(clause);
			pumpStaition.NSDeviceUIDs.Add(pump.UID);
			pumpStaition.NSDevices.Add(pump);
			pumpStaition.Delay = 3;
			GKManager.AddZone(zone);
			GKManager.AddPumpStation(pumpStaition);
			GKManager.AddDeviceToZone(device1, zone);
			GKManager.AddDeviceToZone(device2, zone);
			GKManager.AddDeviceToZone(device3, zone);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Norm, 10000, "Ждем Норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона в Норме");
			TurnOnPim(gkpim);
			TurnOnPim(kaupim);
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка1 у датчика1");
			WaitWhileState(zone, XStateClass.Attention, 4000, "Ждем пока зона не перейдёт во Внимание");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Attention, "Проверка того, что зона во Внимание");
			Assert.IsTrue(pumpStaition.State.StateClass == XStateClass.Off, "Проверка того, что НС Выключен");
			CheckJournal(3, JournalItem(device1, JournalEventNameType.Сработка_1),
				JournalItem(zone, JournalEventNameType.Внимание), JournalItem(Led("Устройство Внимание "), JournalEventNameType.Включено));
			ConrtolGKBase(device2, GKStateBit.Fire1, "Сработка1 у датчика2");
			WaitWhileState(zone, XStateClass.Fire1, 3000, "Ждем пока зона не перейдёт в Пожар");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire1, "Проверка того, что зона перешла в Пожар");
			Assert.IsTrue(pumpStaition.State.StateClass == XStateClass.TurningOn, "Проверка того, что НС Включается");
			WaitWhileState(zone, XStateClass.On, 4000, "Ждем пока НС не Включится");
			Assert.IsTrue(pumpStaition.State.StateClass == XStateClass.On, "Проверка того, что НС Включен");
			Assert.IsTrue(pump.State.StateClass == XStateClass.On, "Проверка того, что насос Включен");
			CheckJournal(8, JournalItem(device2, JournalEventNameType.Сработка_1), JournalItem(Led("Устройство Внимание "), JournalEventNameType.Выключено),
				JournalItem(zone, JournalEventNameType.Пожар_1), JournalItem(pumpStaition, JournalEventNameType.Включается),
				JournalItem(Led("Устройство Включение ПУСК "), JournalEventNameType.Включено), JournalItem(Led("Устройство Пожар 1 "), JournalEventNameType.Включено),
				JournalItem(pumpStaition, JournalEventNameType.Включено), JournalItem(pump, JournalEventNameType.Включено));
		}

		[TestCase(DelayRegime.On, XStateClass.On, JournalEventNameType.Включено)]
		[TestCase(DelayRegime.Off, XStateClass.Off, JournalEventNameType.Выключено)]
		/* RG-1015 (Если у НС задан Режим после удержания "Включено", то после окончания отсчета удержания он не должен переходить в режим "Включается")*/
		public void TestPumpStationDelayRegime(DelayRegime regime, XStateClass state, JournalEventNameType eventname)
		{
			var pumpStaition = new GKPumpStation { Name = "Насосная станция", No = 1 };
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
			var pump = AddDevice(kauDevice11, GKDriverType.RSR2_Bush_Fire);
			var gkpim = AddPim(gkDevice1);
			var kaupim = AddPim(kauDevice11);
			pumpStaition.StartLogic.OnClausesGroup.Clauses.Add(clause);
			pumpStaition.NSDeviceUIDs.Add(pump.UID);
			pumpStaition.NSDevices.Add(pump);
			pumpStaition.Delay = 3;
			pumpStaition.Hold = 3;
			pumpStaition.DelayRegime = regime;
			GKManager.AddZone(zone);
			GKManager.AddPumpStation(pumpStaition);
			GKManager.AddDeviceToZone(device1, zone);
			GKManager.AddDeviceToZone(device2, zone);
			GKManager.AddDeviceToZone(device3, zone);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Norm, 10000, "Ждем Норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона в Норме");
			TurnOnPim(gkpim);
			TurnOnPim(kaupim);
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка1 у датчика1");
			ConrtolGKBase(device2, GKStateBit.Fire1, "Сработка1 у датчика2");
			WaitWhileState(pumpStaition, XStateClass.On, 5000, "Ждем пока НС не Включится");
			Assert.IsTrue(pumpStaition.State.StateClass == XStateClass.On, "Проверка того, что НС Включен");
			WaitWhileState(pumpStaition, XStateClass.TurningOn, 6000, "Ждем 6 секунд, НС не должен перейти в режим Включается");
			Assert.IsFalse(pumpStaition.State.StateClass == XStateClass.TurningOn, "Проверка того, что НС не перешёл в режим Включается");
			Assert.IsTrue(pumpStaition.State.StateClass == state, "Проверка того, что НС Включен/Выключен");
			Assert.IsTrue(pump.State.StateClass == state, "Проверка того, что насос Включен/Выключен");
			WaitWhileState(Led("Устройство Включение ПУСК "), state, 2000,"Ждём пока индикатор не будет Включен/Выключен");
			// RG-1340 CheckJournal(3, JournalItem(pumpStaition, eventname), JournalItem(pump, eventname), JournalItem(Led("Устройство Включение ПУСК "), eventname));
		}
	}
}