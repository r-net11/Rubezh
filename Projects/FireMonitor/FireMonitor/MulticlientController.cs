using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Infrastructure.Common.Windows;
using System.AddIn.Contract;
using System.Windows;
using System.AddIn.Pipeline;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Events;

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
			ServiceFactory.Events.GetEvent<MulticlientStateChanged>().Subscribe(OnMulticlientStateChanged);
		}

		void OnMulticlientStateChanged(StateType stateType)
		{
			if (StateChanged != null)
				StateChanged(stateType);
		}

		public void Start()
		{
			var thread = new Thread(() =>
			{
				ApplicationService.ApplicationController = OnControlChanged;
				var app = new App();
				app.Exit += new ExitEventHandler(app_Exit);
				App.IsMulticlient = true;
				app.InitializeComponent();
				app.Run();
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		public void ShutDown()
		{
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