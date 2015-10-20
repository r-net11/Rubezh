using System;
using System.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.Management;

namespace UinitTestResursNetwork.IncotexNetworkControllerVirtualTests
{
	[TestClass]
	public class IncotexNetworkControllerVirtualTests
	{
		[TestMethod]
		public void StartTest()
		{
			// Arrange
			var controller = new IncotexNetworkControllerVirtual();

			// Action
			controller.Start();
			var status = controller.Status;
			controller.Dispose();

			// Assert
			Assert.AreEqual(Status.Running, status);
		}

		[TestMethod]
		public void SyncDateTimeTest()
		{
			using (ShimsContext.Create())
			{
				// Arrange
				var dt = DateTime.Now;
				var device = new Mercury203Virtual();
				var controller = new IncotexNetworkControllerVirtual();
				controller.Devices.Add(device);
				ShimDateTime.NowGet = () => dt;

				// Action
				controller.SyncDateTime();

				// Assert
				Assert.AreEqual(dt.ToString(), device.RTC.ToString());
			}
		}
	}
}
