using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Contract;

namespace FireMonitor.Multiclient
{
	public class MulticlientControllerWrapper : MarshalByRefObject
	{
		public event Action<INativeHandleContract> ControlChanged;

		public MulticlientController Controller { get; private set; }

		public MulticlientControllerWrapper(MulticlientController controller)
		{
			Controller = controller;
		}
		public void Start()
		{
			Controller.ControlChanged += OnControlChanged;
			Controller.Start();
		}
		public void ShutDown()
		{
			Controller.ShutDown();
		}

		private void OnControlChanged(INativeHandleContract contract)
		{
			ControlChanged(contract);
		}
	}
}
