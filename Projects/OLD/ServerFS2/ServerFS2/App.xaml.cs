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
		const string SignalId = "39967D22-39F1-4472-A254-5F575CB8D18B";
		const string WaitId = "5BDAB105-A425-4A8B-8D23-B5A083A4C904";

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			PatchManager.Patch();

			using (new DoubleLaunchLocker(SignalId, WaitId, true))
			{
				try
				{
					AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
					ThemeHelper.LoadThemeFromRegister();
					FS2LoadHelper.SetLocation(System.Reflection.Assembly.GetExecutingAssembly().Location);
					FS2LoadHelper.SetStatus(FS2State.Opening);
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

		static void Restart()
		{
			Logger.Error("App.Restart");
#if DEBUG
			return;
#endif
			BalloonHelper.ShowFromAgent("Перезапуск");
			var processStartInfo = new ProcessStartInfo()
			{
				FileName = Application.ResourceAssembly.Location,
			};
			System.Diagnostics.Process.Start(processStartInfo);

			Bootstrapper.Close();
			Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
		}
	}
}