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
		/* RG-1324
		Если в однопроходной ТД зажата кнопка на выход, то, при повторном включении ТД, должен включаться отсчёт удержания */
		public void TestOneWayDoorCase1()
		{
			var lockDevice = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var enterDevice = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var exitDevice = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var door = new GKDoor { No = 1, Name = "Точка доступа", DoorType = GKDoorType.OneWay, 
				EnterDevice = enterDevice, EnterDeviceUID = enterDevice.UID, ExitDevice = exitDevice, ExitDeviceUID = exitDevice.UID,
				LockDevice = lockDevice, LockDeviceUID = lockDevice.UID, Hold = 60, Delay = 0};
			GKManager.AddDoor(door);
			SetConfigAndRestartImitator();

			WaitWhileState(door, XStateClass.Off, 10000, "Ждём состояние Закрыто в ТД");
			Assert.IsTrue(door.State.StateClass == XStateClass.Off, "Проверка того, ТД Закрыта");
			ConrtolGKBase(exitDevice, GKStateBit.Fire1, "Зажатие кнопки на выход");
			WaitWhileState(exitDevice, XStateClass.Fire1, 3000, "Ждём Сработку1 кнопки на выход");
			Assert.IsTrue(exitDevice.State.StateClass == XStateClass.Fire1, "Проверка того, что кнопка на выход в Сработке1");
			WaitWhileState(door, XStateClass.On, 3000, "Ждём Включено в ТД");
			Assert.IsTrue(door.State.StateClass == XStateClass.On, "Проверяем, что ТД Включена");
			ConrtolGKBase(door, GKStateBit.TurnOff_InAutomatic, "Выключение ТД");
			WaitWhileState(door, XStateClass.On, 3000, "Ждём Включено в ТД");
			Assert.IsTrue(door.State.StateClass == XStateClass.On, "Проверяем, что ТД Выключена");
// RG 1324			Assert.IsTrue(door.State.HoldDelay > 0, "Проверка того, что ТД на Удeржании");
			Thread.Sleep(1000);
			CheckJournal(7, JournalItem(exitDevice, JournalEventNameType.Сработка_1), JournalItem(door, JournalEventNameType.Включено),
				JournalItem(lockDevice, JournalEventNameType.Включено), JournalItem(door, JournalEventNameType.Выключено),
				JournalItem(lockDevice, JournalEventNameType.Выключено), JournalItem(door, JournalEventNameType.Включено),
				JournalItem(lockDevice, JournalEventNameType.Включено));
		}
	}
}