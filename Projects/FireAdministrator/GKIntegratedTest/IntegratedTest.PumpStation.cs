using System.Linq;
using GKProcessor;
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
			CheckJournal(2, JournalItem(device1, JournalEventNameType.Сработка_1), JournalItem(zone, JournalEventNameType.Внимание));
			ConrtolGKBase(device2, GKStateBit.Fire1, "Сработка1 у датчика2");
			WaitWhileState(zone, XStateClass.Fire1, 3000, "Ждем пока зона не перейдёт в Пожар");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire1, "Проверка того, что зона перешла в Пожар");
			Assert.IsTrue(pumpStaition.State.StateClass == XStateClass.TurningOn, "Проверка того, что НС Включается");
			WaitWhileState(zone, XStateClass.On, 4000, "Ждем пока НС не Включится");
			Assert.IsTrue(pumpStaition.State.StateClass == XStateClass.On, "Проверка того, что НС Включен");
			Assert.IsTrue(pump.State.StateClass == XStateClass.On, "Проверка того, что насос Включен");
			CheckJournal(6, JournalItem(device2, JournalEventNameType.Сработка_1), JournalItem(zone, JournalEventNameType.Пожар_1), JournalItem(pumpStaition, JournalEventNameType.Включается), JournalItem(pumpStaition, JournalEventNameType.Включено), JournalItem(pump, JournalEventNameType.Включено));
		}
	}
}