using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Windows;
using FiresecAPI;
using MuliclientAPI;

namespace FireMonitor.Multiclient
{
	public class MulticlientControllerWrapper : MarshalByRefObject
	{
		public event EventHandler ControlChanged;
		public event Action<StateType> StateTypeChanged;

		FrameworkElement _frameworkElement;
		public string Id { get; private set; }
		public AppDomain AppDomain { get; private set; }
		public MulticlientController Controller { get; private set; }
		public INativeHandleContract Contract { get; private set; }

		public MulticlientControllerWrapper(string id)
		{
			App.Current.Exit += (s, e) => Controller.ShutDown();
			Id = id;
			AppDomain = AppDomain.CreateDomain("Instance" + id);
			Type type = typeof(MulticlientController);
			Controller = (MulticlientController)AppDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
		}
		public void Start(MulticlientData multiclientData)
		{
			Controller.StateChanged += new Action<StateType>(OnStateChanged);
			Controller.ControlChanged += OnControlChanged;
			Controller.Start(multiclientData);
		}

		void OnStateChanged(StateType stateType)
		{
			if (StateTypeChanged != null)
				StateTypeChanged(stateType);
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