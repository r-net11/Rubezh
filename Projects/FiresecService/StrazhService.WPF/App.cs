using System;
using System.Diagnostics;
using System.Windows;
using Common;
using FiresecService;
using FiresecService.Service;
using FiresecService.Service.Validators;
using FiresecService.ViewModels;
using FiresecService.Views;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Theme;
using KeyGenerator;

namespace StrazhService.Starter
{
	public class App : Application
	{
		private const string SignalId = "{59CFC4B4-BA41-4F34-9C41-1CA3851D7019}";
		private const string WaitId = "{6023CC31-322E-4A74-86FD-E851C2E6C20C}";

		[STAThread]
		private static void Main()
		{
			var app = new App();
			Current.Resources.MergedDictionaries.Add(LoadComponent(new Uri("Controls;component/Themes/Styles.xaml", UriKind.Relative)) as ResourceDictionary);
			app.Run();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ThemeHelper.LoadThemeFromRegister();

			try
			{
				PatchManager.Patch();
			}
			catch (Exception)
			{
				MessageBox.Show("Не удалось подключиться к базе данных");
				Current.MainWindow.Close();
			}

			var licenseService = new LicenseManager();

			if (licenseService.IsValidExistingKey())
			{
				Logger.Error("License file is not exists");
			}

			ConfigurationElementsAgainstLicenseDataValidator.Instance.LicenseManager = licenseService;

			// При смене лицензии Сервера производим валидацию конфигурации Сервера на соответствие новой лицензии
			// и уведомляем всех Клиентов
			licenseService.LicenseChanged += () =>
			{
				ConfigurationElementsAgainstLicenseDataValidator.Instance.Validate();
				FiresecServiceManager.SafeFiresecService.NotifyLicenseChanged();
			};

			using (new DoubleLaunchLocker(SignalId, WaitId, true))
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
				try
				{
					
					Bootstrapper.Run(licenseService);
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "App.OnStartup");
					BalloonHelper.ShowFromServer("Ошибка во время загрузки");
					return;
				}
			}
		}

		private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Error((Exception)e.ExceptionObject, "App.CurrentDomain_UnhandledException");
			BalloonHelper.ShowFromServer("Перезагрузка");
			var processStartInfo = new ProcessStartInfo
			{
				FileName = ResourceAssembly.Location
			};
			Process.Start(processStartInfo);
			Bootstrapper.Close();
			Current.MainWindow.Close();
			Current.Shutdown();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			ProcedureRunner.Terminate();
			base.OnExit(e);
		}
	}
}