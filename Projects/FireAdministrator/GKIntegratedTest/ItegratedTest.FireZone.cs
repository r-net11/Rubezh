using System.Collections.Generic;
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
		public void TestFireZoneAttention()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var zone = new GKZone { Name = "Пожарная зона", No = 1 };
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device, zone);
			SetConfigAndRestartImitator();
			WaitWhileState(zone, XStateClass.Norm, 10000, "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона находится в норме");
			ConrtolGKBase(device, GKStateBit.Fire1, "Сработка датчика");
			WaitWhileState(zone, XStateClass.Attention, 3000, "Ждем внимание в зоне");
			CheckJournal(JournalEventNameType.Сработка_1, JournalEventNameType.Внимание);
			Assert.IsTrue(zone.State.StateClass == XStateClass.Attention, "Проверка того, что зона перешла во внимание");
			ConrtolGKBase(device, GKStateBit.Reset, "Сброс датчика");
			WaitWhileState(zone, XStateClass.Norm, 3000, "Ждем норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона перешла в норму");
		}

		[Test]
		public void TestFireZoneFire1()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var zone = new GKZone { Name = "Пожарная зона", No = 1 };
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device1, zone);
			GKManager.AddDeviceToZone(device2, zone);
			SetConfigAndRestartImitator();
			WaitWhileState(zone, XStateClass.Norm, 10000, "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона находится в норме");
			ConrtolGKBase(device1, GKStateBit.Fire1, "Сработка датчика1");
			ConrtolGKBase(device2, GKStateBit.Fire1, "Сработка датчика2");
			WaitWhileState(zone, XStateClass.Fire1, 3000, "Ждем пожар1 в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire1, "Проверка того, что зона перешла в пожар1");
			ConrtolGKBase(device1, GKStateBit.Reset, "Сброс датчика1");
			ConrtolGKBase(device2, GKStateBit.Reset, "Сброс датчика2");
			CheckTime(() => Thread.Sleep(1000), "Ждем 1 секунду");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire1, "Проверка того, что зона еще в состоянии пожар1");
			ConrtolGKBase(zone, GKStateBit.Reset, "Сброс зоны");
			WaitWhileState(zone, XStateClass.Norm, 3000, "Ждем норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона перешла в норму");
		}

		[Test]
		public void TestFireZoneFire2()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_HandDetector);
			var zone = new GKZone { Name = "Пожарная зона", No = 1 };
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device, zone);
			SetConfigAndRestartImitator();
			WaitWhileState(zone, XStateClass.Norm, 10000, "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона находится в норме");
			ConrtolGKBase(device, GKStateBit.Fire2, "Сработка ручника");
			WaitWhileState(zone, XStateClass.Fire2, 3000, "Ждем пожар2 в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire2, "Проверка того, что зона перешла в пожар2");
			ConrtolGKBase(device, GKStateBit.Reset, "Сброс ручника");
			CheckTime(() => Thread.Sleep(1000), "Ждем 1 секунду");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire2, "Проверка того, что зона еще в состоянии пожар2");
			ConrtolGKBase(zone, GKStateBit.Reset, "Сброс зоны");
			WaitWhileState(zone, XStateClass.Norm, 3000, "Ждем норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона перешла в норму");
		}
	}
}
