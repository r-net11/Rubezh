using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
	}
}
