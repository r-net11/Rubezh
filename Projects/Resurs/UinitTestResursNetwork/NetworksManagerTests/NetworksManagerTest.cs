using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using ResursNetwork.Networks;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.Incotex.Models;
using ResursNetwork.OSI.ApplicationLayer;

namespace UinitTestResursNetwork.NetworksManagerTests
{
	[TestClass]
	public class NetworksManagerTest
	{
		internal class TestContener
		{
			NetworksManager _manager;
			ParameterChangedArgs _args = null;

			public ParameterChangedArgs Args
			{
				get { return _args; }
			}

			public bool IsEventRaised
			{
				get { return _args == null ? false : true; }
			}

			public NetworksManager Manager
			{
				get { return _manager; }
				set 
				{ 
					_manager = value;
					_manager.ParameterChanged += EventHandler_manager_ParameterChanged;
				}
			}

			private void EventHandler_manager_ParameterChanged(object sender, 
				ParameterChangedArgs e)
			{
				_args = e;
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
			testCntr.Manager.Networks.Add(controller); //здесь событие не подключается ! исправить !!!
			device.Start();
			controller.Start();

			// Act
			// ждём
			//Thread.Sleep(1000);
			while (!testCntr.IsEventRaised) { Thread.Sleep(1000); }
			controller.Stop();

			// Assert
			Assert.IsTrue(testCntr.IsEventRaised);
		}

		public void SetSatusTest()
		{

		}
	}
}
