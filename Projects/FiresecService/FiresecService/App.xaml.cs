using System;
using System.Diagnostics;
using System.Windows;
using Common;
using FiresecService;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.BalloonTrayTip;
using Infrastructure.Common.Windows.Theme;
using Infrastructure.Automation;

namespace FiresecServiceRunner
{
	public partial class App : Application
	{
		private const string SignalId = "{9C3B6318-48BB-40D0-9249-CA7D9365CDA5}";
		private const string WaitId = "{254FBDB4-7632-42A8-B2C2-27176EF7E60C}";

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ThemeHelper.LoadThemeFromRegister();
			ServerLoadHelper.SetLocation(System.Reflection.Assembly.GetExecutingAssembly().Location);
			ServerLoadHelper.SetStatus(FSServerState.Opening);

			using (new DoubleLaunchLocker(SignalId, WaitId, true))
			{
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
				try
				{
					Bootstrapper.Run();
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "App.OnStartup");
					BalloonHelper.ShowFromServer("Ошибка во время загрузки");
					return;
				}
			}
		}


		protected override void OnExit(ExitEventArgs e)
		{
			AutomationProcessor.Terminate();
			base.OnExit(e);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");
			BalloonHelper.ShowFromServer("Перезагрузка");
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