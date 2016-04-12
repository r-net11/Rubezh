using System.Threading;
using NUnit.Framework;
using RubezhAPI.GK;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace GKIntegratedTest
{
	public partial class ItegratedTest
	{
		[Test]
		public void TestHandDetectorFire2()
		{
			var device = AddDevice(kauDevice11, GKDriverType.RSR2_HandDetector);
			SetConfigAndRestartImitator();
			WaitWhileState(device, XStateClass.Norm, 10000, "Инициализация состояний");
			Assert.IsTrue(device.State.StateClass == XStateClass.Norm, "Проверка того, что ИПР находится в норме");
			ConrtolGKBase(device, GKStateBit.Fire2, "Сработка ручника");
			WaitWhileState(device, XStateClass.Fire2, 3000, "Ждем пожар2 в ИПР");
			Assert.IsTrue(device.State.StateClass == XStateClass.Fire2, "Проверка того, что ИПР перешёл в пожар2");
			CheckTime(() => Thread.Sleep(1000), "Ждем 1 секунду");
			Assert.IsTrue(device.State.StateClass == XStateClass.Fire2, "Проверка того, что ИПР еще в состоянии пожар2");
			ConrtolGKBase(device, GKStateBit.Reset, "Сброс ИПР");
			WaitWhileState(device, XStateClass.Norm, 3000, "Ждем норму в ИПР");
			Assert.IsTrue(device.State.StateClass == XStateClass.Norm, "Проверка того, что ИПР перешла в норму");
		}
	}
}
