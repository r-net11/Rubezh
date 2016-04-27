using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GKIntegratedTest
{
	public partial class ItegratedTest
	{

		[TestCase(GKStateBit.TurnOnNow_InAutomatic, 5)]
		[TestCase(GKStateBit.TurnOn_InAutomatic, 0)]
		/* RG-890 (Включение табличек "Не входи", "Уходи", и "Сирена", если МПТ включается без задержки на включение)*/
		public void TestMptBoardsOnWhenMptTurnOnWithoutDelay(GKStateBit mode, int delay)
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var device3 = AddDevice(kauDevice11, GKDriverType.RSR2_RM_1);
			var mpt = new GKMPT {Name = "МПТ", No = 1, Delay = delay};
			var clause = new GKClause
			{
				ClauseOperationType = ClauseOperationType.AllDevices,
				StateType = GKStateBit.Failure,
				DeviceUIDs = { kauDevice11.UID }
			};
			mpt.MptLogic.OnClausesGroup.Clauses.Add(clause);
			mpt.MPTDevices.Add(new GKMPTDevice { MPTDeviceType = GKMPTDeviceType.DoNotEnterBoard, DeviceUID = device1.UID });
			mpt.MPTDevices.Add(new GKMPTDevice { MPTDeviceType = GKMPTDeviceType.ExitBoard, DeviceUID = device2.UID });
			mpt.MPTDevices.Add(new GKMPTDevice { MPTDeviceType = GKMPTDeviceType.Speaker, DeviceUID = device3.UID });
			GKManager.AddMPT(mpt);
			SetConfigAndRestartImitator();

			WaitWhileState(mpt, XStateClass.Off, 10000, "Ждем Норму в МПТ");
			Assert.IsTrue(mpt.State.StateClass == XStateClass.Off, "Проверка того, что МПТ Выключен");
			ConrtolGKBase(mpt, mode, "Включаем МПТ");
			WaitWhileState(mpt, XStateClass.On, 3000, "Ждем Включено в МПТ");
			Assert.IsTrue(mpt.State.StateClass == XStateClass.On, "Проверка того, что МПТ Включен");
			Assert.IsTrue(device1.State.StateClass == XStateClass.On, "Проверка того, что Табличка Не Входи Включена");
			Assert.IsTrue(device2.State.StateClass == XStateClass.On, "Проверка того, что Табличка Уходи Включена");
			Assert.IsTrue(device3.State.StateClass == XStateClass.On, "Проверка того, что Сирена Включена");
			CheckJournal(5, JournalItem(mpt, JournalEventNameType.Включено), JournalItem(device1, JournalEventNameType.Включено),
				JournalItem(device2, JournalEventNameType.Включено), JournalItem(device3, JournalEventNameType.Включено),
				JournalItem(Led("Устройство Включение ПУСК "), JournalEventNameType.Включено));
		}
	}
}