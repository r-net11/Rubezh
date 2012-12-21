using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Infrastructure.Common.Windows;
using System.AddIn.Contract;
using System.Windows;
using System.AddIn.Pipeline;

namespace FireMonitor
{
	public class MulticlientController : MarshalByRefObject
	{
		public event Action<INativeHandleContract> ControlChanged;

		public void Start()
		{
			Thread thread = new Thread(() =>
			{
				ApplicationService.ApplicationController = OnControlChanged;
				App app = new App();
				app.Exit += new ExitEventHandler(app_Exit);
				app.IsMulticlient = true;
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
		private void app_Exit(object sender, ExitEventArgs e)
		{
		}
	}
}
