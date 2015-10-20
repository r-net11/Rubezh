using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using ResursNetwork.Networks;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.Incotex.Models;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.Management;

namespace UinitTestResursNetwork.NetworksManagerTests
{
	[TestClass]
	public class NetworksManagerTest
	{
		internal class TestContener
		{
			NetworksManager _manager;
			ParameterChangedArgs _paramChangedArgs = null;
			StatusChangedEventArgs _statusChangedArgs = null;

			public ParameterChangedArgs ParamChangedArgs
			{
				get { return _paramChangedArgs; }
			}

			public StatusChangedEventArgs StatusChangedArgs
			{
				get { return _statusChangedArgs; }
			}

			public bool IsEventRaisedParamChanged
			{
				get { return _paramChangedArgs == null ? false : true; }
			}

			public bool IsEventRaisedStatusChanged
			{
				get { return _statusChangedArgs == null ? false : true; }
			}

			public NetworksManager Manager
			{
				get { return _manager; }
				set 
				{ 
					_manager = value;
					_manager.ParameterChanged += EventHandler_manager_ParameterChanged;
					_manager.StatusChanged += EventHandler_StatusChanged;
				}
			}

			public void EventHandler_StatusChanged(object sender, StatusChangedEventArgs e)
			{
				_statusChangedArgs = e;
			}

			private void EventHandler_manager_ParameterChanged(object sender, 
				ParameterChangedArgs e)
			{
				_paramChangedArgs = e;
			}

		}

		/// <summary>
		/// Проверяем генерацию события ParameterChanged
		/// контроллером сети IncotexNetworkControllerVirtual
		/// </summary>
		[TestMethod]
		public void RaiseEventParameterChangedByIncotexNetworkControllerVirtualTest()
		{
			// Arrange
			var testCntr = new TestContener();
			testCntr.Manager = NetworksManager.Instance;
			var controller = new IncotexNetworkControllerVirtual();
			var device = new Mercury203Virtual();

			controller.Devices.Add(device);
			testCntr.Manager.Networks.Add(controller);
			device.Start();
			controller.Start();

			// Act
			// ждём
			Thread.Sleep(1000);
			//while (!testCntr.IsEventRaised) { Thread.Sleep(1000); }
			controller.Stop();

			// Assert
			Assert.IsTrue(testCntr.IsEventRaisedParamChanged);
		}

		[TestMethod]
		public void SetStatusTest()
		{
			// arrange
			var manager = NetworksManager.Instance;
			var controller = new IncotexNetworkControllerVirtual();
			var device = new Mercury203Virtual();

			controller.Devices.Add(device);
			manager.Networks.Add(controller);

			// Проверяем установку статуса сонтроллера сети
			// Act
			manager.SetSatus(controller.Id, true);

			// Assert
			Assert.AreEqual(Status.Running, controller.Status);

			// Act
			manager.SetSatus(controller.Id, false);
			Thread.Sleep(2000); // Ждём завершения

			// Assert
			Assert.AreEqual(Status.Stopped, controller.Status);

			// Проверяем установку статуса устройтсва
			// Act
			manager.SetSatus(device.Id, true);

			// Assert
			Assert.AreEqual(Status.Running, device.Status);

			// Act
			manager.SetSatus(device.Id, false);

			// Assert
			Assert.AreEqual(Status.Stopped, device.Status);
		}

		/// <summary>
		/// Проверяем работу события StatusChanged на вирутальном контроллере
		/// и виртуальном устройстве
		/// </summary>
		[TestMethod]
		public void RaiseEventStatusChangedTest()
		{
			// Arrange
			var testCntr = new TestContener();
			testCntr.Manager = NetworksManager.Instance;
			var controller = new IncotexNetworkControllerVirtual();
			var device = new Mercury203Virtual();

			controller.Devices.Add(device);
			testCntr.Manager.Networks.Add(controller);

			// Act
			testCntr.Manager.SetSatus(controller.Id, true);

			// Assert
			Assert.IsTrue(testCntr.IsEventRaisedStatusChanged);
			Assert.AreEqual(Status.Running, controller.Status);
			Assert.AreEqual(controller.Id, testCntr.StatusChangedArgs.Id);
			Assert.AreEqual(controller.Status, testCntr.StatusChangedArgs.Status);

			// Act
			testCntr.Manager.SetSatus(controller.Id, false);

			// Assert
			Assert.IsTrue(testCntr.IsEventRaisedStatusChanged);
			Assert.AreEqual(Status.Stopped, controller.Status);
			Assert.AreEqual(controller.Id, testCntr.StatusChangedArgs.Id);
			Assert.AreEqual(controller.Status, testCntr.StatusChangedArgs.Status);
		}
	}
}
