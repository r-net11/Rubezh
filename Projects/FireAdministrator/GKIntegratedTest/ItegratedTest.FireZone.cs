using System.Threading;
using NUnit.Framework;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhClient;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GKIntegratedTest
{
	public partial class ItegratedTest
	{
		[Test]
		[Category("Integration")]
		public void TestFireZoneAttention()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var zone = new GKZone { Name = "Пожарная зона", No = 1 };
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device, zone);
			SetConfigAndRestartImitator();
			CheckTime(() => WaitWhileState(zone, XStateClass.Norm, 10000), "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона находится в норме");
			CheckTime(() => ConrtolGKBase(device, GKStateBit.Fire1), "Сработка датчика");
			CheckTime(() => WaitWhileState(zone, XStateClass.Attention, 3000), "Ждем внимание в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Attention, "Проверка того, что зона перешла во внимание");
			CheckTime(() => ConrtolGKBase(device, GKStateBit.Reset), "Сброс датчика");
			CheckTime(() => WaitWhileState(zone, XStateClass.Norm, 3000), "Ждем норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона перешла в норму");
		}

		[Test]
		[Category("Integration")]
		public void TestFireZoneFire1()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_SmokeDetector);
			var zone = new GKZone { Name = "Пожарная зона", No = 1 };
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device1, zone);
			GKManager.AddDeviceToZone(device2, zone);
			SetConfigAndRestartImitator();
			CheckTime(() => WaitWhileState(zone, XStateClass.Norm, 10000), "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона находится в норме");
			CheckTime(() => ConrtolGKBase(device1, GKStateBit.Fire1), "Сработка датчика1");
			CheckTime(() => ConrtolGKBase(device2, GKStateBit.Fire1), "Сработка датчика2");
			CheckTime(() => WaitWhileState(zone, XStateClass.Fire1, 3000), "Ждем пожар1 в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire1, "Проверка того, что зона перешла в пожар1");
			CheckTime(() => ConrtolGKBase(device1, GKStateBit.Reset), "Сброс датчика1");
			CheckTime(() => ConrtolGKBase(device2, GKStateBit.Reset), "Сброс датчика2");
			CheckTime(() => Thread.Sleep(1000), "Ждем 1 секунду");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire1, "Проверка того, что зона еще в состоянии пожар1");
			CheckTime(() => ConrtolGKBase(zone, GKStateBit.Reset), "Сброс зоны");
			CheckTime(() => WaitWhileState(zone, XStateClass.Norm, 3000), "Ждем норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона перешла в норму");
		}

		[Test]
		[Category("Integration")]
		public void TestFireZoneFire2()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_HandDetector);
			var zone = new GKZone { Name = "Пожарная зона", No = 1 };
			GKManager.AddZone(zone);
			GKManager.AddDeviceToZone(device, zone);
			SetConfigAndRestartImitator();
			CheckTime(() => WaitWhileState(zone, XStateClass.Norm, 10000), "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона находится в норме");
			CheckTime(() => ConrtolGKBase(device, GKStateBit.Fire2), "Сработка ручника");
			CheckTime(() => WaitWhileState(zone, XStateClass.Fire2, 3000), "Ждем пожар2 в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire2, "Проверка того, что зона перешла в пожар2");
			CheckTime(() => ConrtolGKBase(device, GKStateBit.Reset), "Сброс ручника");
			CheckTime(() => Thread.Sleep(1000), "Ждем 1 секунду");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Fire2, "Проверка того, что зона еще в состоянии пожар2");
			CheckTime(() => ConrtolGKBase(zone, GKStateBit.Reset), "Сброс зоны");
			CheckTime(() => WaitWhileState(zone, XStateClass.Norm, 3000), "Ждем норму в зоне");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Norm, "Проверка того, что зона перешла в норму");
		}
	}
}
