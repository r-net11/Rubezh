using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Windows;
using Infrastructure.Common.Windows;

namespace FireMonitor.Multiclient
{
	public class MulticlientControllerWrapper : MarshalByRefObject
	{
		public event EventHandler ControlChanged;

		FrameworkElement _frameworkElement;
		public int Index { get; private set; }
		public AppDomain AppDomain { get; private set; }
		public MulticlientController Controller { get; private set; }
		public INativeHandleContract Contract { get; private set; }

		public MulticlientControllerWrapper(int index)
		{
			App.Current.Exit += (s, e) => Controller.ShutDown();
			Index = index;
			AppDomain = AppDomain.CreateDomain("Instance" + index.ToString());
			Type type = typeof(MulticlientController);
			Controller = (MulticlientController)AppDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
		}
		public void Start()
		{
			Controller.StateChanged += new Action<FiresecAPI.StateType>(OnStateChanged);
			Controller.ControlChanged += OnControlChanged;
			Controller.Start();
		}

		void OnStateChanged(FiresecAPI.StateType obj)
		{
			;
		}

		public void ShutDown()
		{
			Controller.ShutDown();
		}

		void OnControlChanged(INativeHandleContract contract)
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