using System.Threading;
using GKProcessor;
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
		//RG-1223
		public void TestGuardZoneIsOnAfterResetCasetWithCardReader()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_CardReader);
			var cardReader = new GKGuardZoneDevice { Device = device1, DeviceUID = device1.UID };
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM_1 = new GKGuardZoneDevice { Device = device2, DeviceUID = device2.UID };
			aM_1.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			var device3 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var guardDetector = new GKGuardZoneDevice { Device = device3, DeviceUID = device3.UID };
			guardDetector.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			var zone = new GKGuardZone { Name = "Охранная зона", No = 1 };
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, aM_1);
			GKManager.AddDeviceToGuardZone(zone, cardReader);
			GKManager.AddDeviceToGuardZone(zone, guardDetector);
			SetConfigAndRestartImitator();

			CheckTime(() => WaitWhileState(zone, XStateClass.Off, 10000), "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Off, "Проверка того, что зона снята с охраны");
			CheckTime(() => ConrtolGKBase(zone, GKStateBit.TurnOn_InAutomatic), "Постановка зоны на охрану");
			CheckTime(() => WaitWhileState(zone, XStateClass.On, 3000), "Ждем пока зона не встанет на охрану");
			Assert.IsTrue(zone.State.StateClass == XStateClass.On, "Проверка того, что зона установилась на охрану");
			CheckTime(() => ConrtolGKBase(device3, GKStateBit.Fire1), "Сработка тревоги у датчика");
			CheckTime(() => WaitWhileState(zone, XStateClass.Fire1, 10000), "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");
			CheckTime(() => Thread.Sleep(1000), "Ждем 1 секунду");
			Assert.IsTrue(device3.State.StateClass == XStateClass.Fire1, "Проверка того, что датчик ещё в тревоге");
			CheckTime(() => ConrtolGKBase(zone, GKStateBit.Reset), "Cброс зоны");
			CheckTime(() => WaitWhileState(zone, XStateClass.Fire1, 10000), "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");

		}
		[Test]
		[Category("Integration")]
		//RG-1223
		public void TestGuardZoneIsOnAfterResetCasetWithoutCardReader()
		{
			var device1 = AddDevice(kauDevice11, GKDriverType.RSR2_AM_1);
			var aM_1 = new GKGuardZoneDevice { Device = device1, DeviceUID = device1.UID };
			aM_1.ActionType = GKGuardZoneDeviceActionType.ChangeGuard;
			var device2 = AddDevice(kauDevice11, GKDriverType.RSR2_GuardDetector);
			var guardDetector = new GKGuardZoneDevice { Device = device2, DeviceUID = device2.UID };
			guardDetector.ActionType = GKGuardZoneDeviceActionType.SetAlarm;
			var zone = new GKGuardZone { Name = "Охранная зона", No = 1 };
			GKManager.AddGuardZone(zone);
			GKManager.AddDeviceToGuardZone(zone, aM_1);
			GKManager.AddDeviceToGuardZone(zone, guardDetector);
			SetConfigAndRestartImitator();

			CheckTime(() => WaitWhileState(zone, XStateClass.Off, 10000), "Инициализация состояний");
			Assert.IsTrue(zone.State.StateClass == XStateClass.Off, "Проверка того, что зона снята с охраны");
			CheckTime(() => ConrtolGKBase(zone, GKStateBit.TurnOn_InAutomatic), "Постановка зоны на охрану");
			CheckTime(() => WaitWhileState(zone, XStateClass.On, 3000), "Ждем пока зона не встанет на охрану");
			Assert.IsTrue(zone.State.StateClass == XStateClass.On, "Проверка того, что зона установилась на охрану");
			CheckTime(() => ConrtolGKBase(device2, GKStateBit.Fire1), "Сработка тревоги у датчика");
			CheckTime(() => WaitWhileState(zone, XStateClass.Fire1, 10000), "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");
			CheckTime(() => Thread.Sleep(1000), "Ждем 1 секунду");
			Assert.IsTrue(device2.State.StateClass == XStateClass.Fire1, "Проверка того, что датчик ещё в тревоге");
			CheckTime(() => ConrtolGKBase(zone, GKStateBit.Reset), "Cброс зоны");
			CheckTime(() => WaitWhileState(zone, XStateClass.Fire1, 10000), "Ждем пока зона не перейдёт в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.Fire1), "Проверка того, что зона перешла в тревогу");
			Assert.IsTrue(zone.State.StateClasses.Contains(XStateClass.On), "Проверка того, что зона на охране");
		}
	}
}