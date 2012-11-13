using System;
using System.Windows;
using Common;
using FiresecService;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
using System.Diagnostics;

namespace FiresecServiceRunner
{
    public partial class App : Application
    {
        private const string SignalId = "{9C3B6318-48BB-40D0-9249-CA7D9365CDA5}";
        private const string WaitId = "{254FBDB4-7632-42A8-B2C2-27176EF7E60C}";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            ThemeHelper.LoadThemeFromRegister();

            using (new DoubleLaunchLocker(SignalId, WaitId, true))
                Bootstrapper.Run();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");

			var processStartInfo = new ProcessStartInfo()
			{
				FileName = Application.ResourceAssembly.Location
			};
			System.Diagnostics.Process.Start(processStartInfo);

			Application.Current.MainWindow.Close();
			Application.Current.Shutdown();
        }
    }
}