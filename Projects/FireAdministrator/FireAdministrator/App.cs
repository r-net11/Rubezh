using Controls;
using Infrastructure;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Theme;
using Infrastructure.Common.Windows;
using RubezhClient;
using System;
using System.ComponentModel;
using System.Windows;

namespace FireAdministrator
{
	public class App : Application
	{
		const string SignalId = "{8599F876-2147-4694-A822-B24E36D7F92F}";
		const string WaitId = "{07193C2C-CE04-478C-880A-49AB239C6550}";
		Bootstrapper _bootstrapper;

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
			finally
			{
				ServiceFactory.StartupService.Close();
			}
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBoxService.ShowException(e.ExceptionObject as Exception);
			if (MessageBoxService.ShowQuestion("В результате работы программы произошло исключение. Приложение будет закрыто. Вы хотите сохранить конфигурацию в файл"))
			{
				FileConfigurationHelper.SaveToFile();
			}
		}
		void ApplicationService_Closing(object sender, CancelEventArgs e)
		{
			if (e.Cancel)
				return;

			if (ApplicationService.Modules != null)
				foreach (var module in ApplicationService.Modules)
					module.Dispose();
			ClientManager.Disconnect();
		}

		[STAThread]
		static void Main()
		{
			ServiceFactory.StartupService.Run();
			var app = new App();
			ServiceFactory.ResourceService.AddResource(typeof(UIBehavior).Assembly, "Themes/Styles.xaml");
			app.Run();
		}
	}
}