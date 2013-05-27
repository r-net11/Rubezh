using System;
using System.Diagnostics;
using System.Windows;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Theme;

namespace ServerFS2
{
	public partial class App : Application
	{
		private const string SignalId = "39967D22-39F1-4472-A254-5F575CB8D18B";
		private const string WaitId = "5BDAB105-A425-4A8B-8D23-B5A083A4C904";

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			PatchManager.Patch();
			Microsoft.Win32.SystemEvents.SessionEnding += new Microsoft.Win32.SessionEndingEventHandler(SystemEvents_SessionEnding);

			using (new DoubleLaunchLocker(SignalId, WaitId, true))
			{
				try
				{
					AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
					ThemeHelper.LoadThemeFromRegister();
					FSAgentLoadHelper.SetLocation(System.Reflection.Assembly.GetExecutingAssembly().Location);
					FSAgentLoadHelper.SetStatus(FSAgentState.Opening);
					Bootstrapper.Run();
				}
				catch (Exception ex)
				{
					BalloonHelper.ShowFromAgent("Ошибка во время загрузки");
					Logger.Error(ex, "App.OnStartup");
				}
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error(e.ExceptionObject as Exception, "App.CurrentDomain_UnhandledException");
			Restart();
		}

		public static void Restart()
		{
			Logger.Error("App.Restart");
#if DEBUG
			return;
#endif
			BalloonHelper.ShowFromAgent("Перезапуск");
			Bootstrapper.Close();
			var processStartInfo = new ProcessStartInfo()
			{
				FileName = Application.ResourceAssembly.Location,
			};
			System.Diagnostics.Process.Start(processStartInfo);

			Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
		}

		private static void SystemEvents_SessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
		{
			CloseOnComputerShutdown(true);
		}

		public static void CloseOnComputerShutdown(bool isShuttingDown)
		{
			Bootstrapper.Close();
			if (isShuttingDown)
			{
				ShutDownComputer();
			}
			Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
		}

		private static void ShutDownComputer()
		{
			if (GlobalSettingsHelper.GlobalSettings.ForceShutdown)
			{
				var processStartInfo = new ProcessStartInfo()
				{
					FileName = "shutdown.exe",
					Arguments = "/s /t 00 /f",
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden
				};
				Process.Start(processStartInfo);
			}
		}
	}
}