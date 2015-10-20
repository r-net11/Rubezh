using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursAPI.ParameterNames;
using System.Threading;

namespace UinitTestResursNetwork.VirtualIncotexNetworkControllerVirtualTests
{
	[TestClass]
	public class Mercury203VirtualTests
	{
		[TestMethod]
		public void WriteCounterTarifTest()
		{
			// arrange
			var device = new Mercury203Virtual();
			
			// act
			UInt32 tarif1 = 10, tarif2 = 12, tarif3 = 14, tarif4 = 15;

			device.Parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value = tarif1;
			device.Parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value = tarif2;
			device.Parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value = tarif3;
			device.Parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value = tarif4; 

			// assert
			Assert.AreEqual(tarif1, device.Parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value);
			Assert.AreEqual(tarif2, device.Parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value);
			Assert.AreEqual(tarif3, device.Parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value);
			Assert.AreEqual(tarif4, device.Parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value);
		}

		[TestMethod]
		public void WorkingCounterTest()
		{ 
			// arrange
			UInt32 tarif1 = 10, tarif2 = 12, tarif3 = 14, tarif4 = 15;
			var device = new Mercury203Virtual();
			
			device.Parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value = tarif1;
			device.Parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value = tarif2;
			device.Parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value = tarif3;
			device.Parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value = tarif4;
 
			var controller = new IncotexNetworkControllerVirtual();
			controller.Devices.Add(device);

			// act
			controller.Start();
			Thread.Sleep(5000);

			// assert
			Assert.IsTrue(tarif1 < (UInt32)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value, 
				"Значение счётчика должно было измениться в большую сторону");
			Assert.IsTrue(tarif2 < (UInt32)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value,
				"Значение счётчика должно было измениться в большую сторону");
			Assert.IsTrue(tarif3 < (UInt32)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value,
				"Значение счётчика должно было измениться в большую сторону");
			Assert.IsTrue(tarif4 < (UInt32)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value,
				"Значение счётчика должно было измениться в большую сторону");
			
			controller.Dispose();
		}
	}
}
