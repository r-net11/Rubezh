using System;
using System.ComponentModel;
using System.Windows;
using RubezhClient;
using Infrastructure;
using Infrastructure.Client.Startup;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Theme;
using Infrastructure.Common.Windows.Windows;

namespace FireAdministrator
{
	public partial class App : Application
	{
		private const string SignalId = "{8599F876-2147-4694-A822-B24E36D7F92F}";
		private const string WaitId = "{07193C2C-CE04-478C-880A-49AB239C6550}";
		private Bootstrapper _bootstrapper;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			try
			{
				string fileName;
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
				ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
				ThemeHelper.LoadThemeFromRegister();
#if DEBUG
				bool trace = false;
				BindingErrorListener.Listen(m => { if (trace) MessageBox.Show(m); });
#endif
				_bootstrapper = new Bootstrapper();
				using (new DoubleLaunchLocker(SignalId, WaitId))
					_bootstrapper.Initialize();
				if (Application.Current != null && e.Args != null && e.Args.Length > 0)
				{
					fileName = e.Args[0];
					FileConfigurationHelper.LoadFromFile(fileName);
				}
			}
			catch (StartupCancellationException)
			{
				ApplicationService.ShutDown();
				return;
			}
			finally
			{
				ServiceFactory.StartupService.Close();
			}
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBoxService.ShowException(e.ExceptionObject as Exception);
			if (MessageBoxService.ShowQuestion("В результате работы программы произошло исключение. Приложение будет закрыто. Вы хотите сохранить конфигурацию в файл"))
			{
				FileConfigurationHelper.SaveToFile();
			}
		}
		private void ApplicationService_Closing(object sender, CancelEventArgs e)
		{
			if (e.Cancel)
				return;

			if (ApplicationService.Modules != null)
				foreach (var module in ApplicationService.Modules)
					module.Dispose();
			ClientManager.Disconnect();
		}

		[STAThread]
		private static void Main()
		{
			ServiceFactory.StartupService.Run();
			var app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}