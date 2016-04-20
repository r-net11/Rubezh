using System;
using System.Diagnostics;
using System.Windows;
using Common;
using Resurs;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.BalloonTrayTip;
using Infrastructure.Common.Windows.Theme;
using System.Threading;
using Infrastructure.Common.Windows.Windows;
using Resurs.Processor;

namespace ResursRunner
{
	public partial class App : Application
	{
		private const string SignalId = "8DC89238-5FD3-4631-8D50-96B4FF7AA7DC";
		private const string WaitId = "0769AC75-8AF6-40DE-ABDA-83C1E3C7ABFF";
		
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ServerLoadHelper.SetLocation(System.Reflection.Assembly.GetExecutingAssembly().Location);
			
			var showWindow = true;
			if(e.Args.Length > 0)
			{
				showWindow = e.Args[0].ToLower() != "-hide";
			}

			using (new SingleLaunchActivator(SignalId, WaitId, OnShuttingDown, OnActivate))
			{
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
				try
				{
					Bootstrapper.Run(showWindow);
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "App.OnStartup");
					BalloonHelper.Show("АРМ Ресурс", "Ошибка во время загрузки");
					return;
				}
			}
		}

		void OnActivate()
		{
			ApplicationService.Invoke(Bootstrapper.Activate);
		}

		void OnShuttingDown()
		{
			;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");
			BalloonHelper.Show("АРМ Ресурс", "Перезагрузка");
			var processStartInfo = new ProcessStartInfo()
			{
				FileName = Application.ResourceAssembly.Location
			};
			System.Diagnostics.Process.Start(processStartInfo);
			Bootstrapper.Close();
			Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
		}
	}
}