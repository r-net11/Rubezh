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
		//[TestCase(GKDriverType.RSR2_Button)]
		/* Тестирование жизненного цикла датчиков и пожарной зоны в режиме Сработка1 */
		public void TestDeviceFire1(GKDriverType driver)
		{
			var device1 = AddDevice(kauDevice11, driver);
			var device2 = AddDevice(kauDevice11, driver);
			var device3 = AddDevice(kauDevice11, driver);
			var zone = new GKZone { Name = "Пожарная зона", No = 1 };
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device1,zone);
			GKManager.AddDeviceToZone(device2, zone);
			GKManager.AddDeviceToZone(device3, zone);
			SetConfigAndRestartImitator();

			WaitWhileState(zone, XStateClass.Norm, 10000, "Ждём Норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона находится в Норме");
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка1 в device");
			WaitWhileState(device1, XStateClass.Fire1, 3000, "Ожидаем пока device1 перейдёт в Сработка1");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Fire1, "Проверка того, что device1 находится в Сработка1");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Attention, "Проверка того, что пожараная зона перешла во Внимание");
			CheckJournal(2, JournalItem(device1, JournalEventNameType.Сработка_1), JournalItem(zone, JournalEventNameType.Внимание));
			ConrtolGKBase(device2, GKStateBit.Fire1, "Сработка1 в device2");
			WaitWhileState(device2, XStateClass.Fire1, 3000, "Ожидаем пока device2 перейдёт в Сработка1");
			Assert.IsTrue(device2.State.StateClass == XStateClass.Fire1, "Проверка того, что device2 находится в Сработка1");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire1, "Проверка того, что пожараная зона перешла в Пожар1");
			CheckJournal(2, JournalItem(device2, JournalEventNameType.Сработка_1), JournalItem(zone, JournalEventNameType.Пожар_1));
			ConrtolGKBase(device1, GKStateBit.Reset, "Сброс device1");
			ConrtolGKBase(zone, GKStateBit.Reset, "Сброс зоны");
			WaitWhileState(device1, XStateClass.Norm, 3000, "Ожидаем пока device1 перейдёт в Норму");
			Assert.IsTrue(device1.State.StateClass == XStateClass.Norm, "Проверка того, что device1 перешёл в Норму");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Attention), "Проверка того, что зона перешла во Внимание");
			CheckJournal(3, JournalItem(device1, JournalEventNameType.Норма), JournalItem(zone, JournalEventNameType.Норма), JournalItem(zone, JournalEventNameType.Внимание));
			ConrtolGKBase(device2, GKStateBit.Reset, "Сброс device2");
			WaitWhileState(device2, XStateClass.Norm, 3000, "Ожидаем пока device2 перейдёт в Норму");
			Assert.IsTrue(device2.State.StateClass == XStateClass.Norm, "Проверка того, что device2 перешёл в Норму");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона перешла в Норму");
			CheckJournal(3, JournalItem(device2, JournalEventNameType.Норма), JournalItem(zone, JournalEventNameType.Норма));
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
			var zone = new GKZone { Name = "Пожарная зона", No = 1 };
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
			CheckJournal(2, JournalItem(device1, JournalEventNameType.Сработка_2), JournalItem(zone, JournalEventNameType.Пожар_2));
			ConrtolGKBase(zone, GKStateBit.Reset, "Сброс зоны");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire2, "Проверка того, что зона всё ещё в режиме Сработка2");
			ConrtolGKBase(device1, GKStateBit.Reset, "Сброс устройства1");
			WaitWhileState(device1, XStateClass.Norm, 3000, "Ожидаем пока device1 перейдёт в Норму");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire2, "Проверка того, что зона всё ещё в режиме Сработка2");
			CheckJournal(1, JournalItem(device1, JournalEventNameType.Норма));
			ConrtolGKBase(zone, GKStateBit.Reset, "Сброс зоны");
			WaitWhileState(zone, XStateClass.Norm, 3000, "Ожидаем пока зона перейдёт в Норму");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона перешла в Норму");
			CheckJournal(1, JournalItem(zone, JournalEventNameType.Норма));
		}
	}
}
