using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Contract;
using System.Windows;
using System.AddIn.Pipeline;

namespace FireMonitor.Multiclient
{
	public class MulticlientControllerWrapper : MarshalByRefObject
	{
		public event EventHandler ControlChanged;

		private AppDomain _domain;
		private FrameworkElement _frameworkElement;
		public int Index { get; private set; }
		public MulticlientController Controller { get; private set; }
		public INativeHandleContract Contract { get; private set; }

		public MulticlientControllerWrapper(int index)
		{
			App.Current.Exit += (s, e) => Controller.ShutDown();
			Index = index;
			_domain = AppDomain.CreateDomain("Instance" + index.ToString());
			Type type = typeof(MulticlientController);
			Controller = (MulticlientController)_domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
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
			Contract = contract;
			_frameworkElement = null;
			if (ControlChanged != null)
				ControlChanged(this, EventArgs.Empty);
		}
		public override object InitializeLifetimeService()
		{
			return null;
		}

		public FrameworkElement GetContent()
		{
			if (_frameworkElement == null && Contract != null)
				_frameworkElement = GetContent(Contract);
			return _frameworkElement;
		}
		public static FrameworkElement GetContent(INativeHandleContract contract)
		{
			return FrameworkElementAdapters.ContractToViewAdapter(contract);
		}
	}
}
