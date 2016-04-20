using Common;
using FiresecService;
using Infrastructure.Automation;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Theme;
using System;
using System.Diagnostics;
using System.Windows;

namespace FiresecServiceRunner
{
	public class App : Application
	{
		const string SignalId = "{9C3B6318-48BB-40D0-9249-CA7D9365CDA5}";
		const string WaitId = "{254FBDB4-7632-42A8-B2C2-27176EF7E60C}";

		[STAThread]
		static void Main()
		{
			App app = new App();
			Current.Resources.MergedDictionaries.Add(LoadComponent(
													 new Uri("Controls;component/Themes/Styles.xaml", UriKind.Relative)) as ResourceDictionary);
			app.Run();
		}

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

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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