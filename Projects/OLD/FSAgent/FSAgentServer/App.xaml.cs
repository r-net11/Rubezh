using System;
using System.Diagnostics;
using System.Windows;
using Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.BalloonTrayTip;
using Infrastructure.Common.Windows.Theme;

namespace FSAgentServer
{
	public partial class App : Application
	{
		const string SignalId = "7D46A5A4-AC89-4F36-A834-1070CFCFF609";
		const string WaitId = "A64BC0A9-319C-4028-B666-5CE56BFD1B1B";

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

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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

		static void SystemEvents_SessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
		{
			CloseOnComputerShutdown(true);
		}

		public static void CloseOnComputerShutdown(bool isShuttingDown)
		{
			Bootstrapper.Close();
			ShutDownComServer();
			Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
		}

		static void ShutDownComServer()
		{
			SocketServerHelper.Stop();
			var processes = Process.GetProcessesByName("FS_SER~1.EXE");
			if (processes != null)
			{
				foreach (var process in processes)
				{
					process.Kill();
				}
			}
		}
	}
}