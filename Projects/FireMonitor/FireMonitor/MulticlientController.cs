using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Threading;
using System.Windows;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using MuliclientAPI;

namespace FireMonitor
{
	public class MulticlientController : MarshalByRefObject
	{
		public event Action<StateType> StateChanged;
		public event Action<INativeHandleContract> ControlChanged;
		public static MulticlientController Current;

		public MulticlientController()
		{
			Current = this;
		}
		public void SuscribeMulticlientStateChanged()
		{
			ServiceFactory.Events.GetEvent<MulticlientStateChangedEvent>().Subscribe(OnMulticlientStateChanged);
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Subscribe(OnDevicesStateChanged);
			OnDevicesStateChanged(null);
		}

		void OnDevicesStateChanged(object obj)
		{
			var minState = StateType.Norm;
			foreach (var device in FiresecManager.Devices)
			{
				foreach (var state in device.DeviceState.ThreadSafeStates)
				{
					if (state.DriverState.StateType < minState)
						minState = state.DriverState.StateType;
				}
			}
			OnMulticlientStateChanged(minState);
		}

		void OnMulticlientStateChanged(StateType stateType)
		{
			if (StateChanged != null)
				StateChanged(stateType);
		}

		public void Start(MulticlientData multiclientData)
		{
			var thread = new Thread(() =>
			{
				ApplicationService.ApplicationController = OnControlChanged;
				var app = new App();
				app.Exit += new ExitEventHandler(app_Exit);
				app.SetMulticlientData(multiclientData);
				app.InitializeComponent();
				app.Run();
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		public void ShutDown()
		{
			Environment.Exit(0);
			ApplicationService.ShutDown();
		}

		private void OnControlChanged(FrameworkElement frameworkElement)
		{
			ApplicationService.Invoke(() =>
				{
					var contract = FrameworkElementAdapters.ViewToContractAdapter(frameworkElement);
					if (ControlChanged != null)
						ControlChanged(contract);
				});
		}
		void app_Exit(object sender, ExitEventArgs e)
		{
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}
	}
}