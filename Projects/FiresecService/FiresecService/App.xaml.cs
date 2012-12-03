using System;
using System.Windows;
using Common;
using FiresecService;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
using System.Diagnostics;
using Infrastructure.Common.BalloonTrayTip;

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
            ServerLoadHelper.SetLocation();
            ServerLoadHelper.SetStatus(FSServerState.Opening);

            using (new DoubleLaunchLocker(SignalId, WaitId, true))
            {
                try
                {
                    Bootstrapper.Run();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "App.OnStartup");
                    BalloonHelper.ShowWarning("Сервер приложений Firesec", "Ошибка во время загрузки");
                }
            }
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");
            BalloonHelper.ShowWarning("Сервер приложений Firesec", "Перезагрузка");
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