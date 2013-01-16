using System;
using System.IO;
using System.Windows;
using FireMonitor.Multiclient.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace FireMonitor.Multiclient
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ServiceFactory.Initialize();

			if (!LicenseHelper.CheckLicense(3))
			{
				MessageBoxService.ShowError("Отсутстует лицензия. Приложение будет закрыто");
				Shutdown();
				return;
			}

			var multiclientViewModel = new MulticlientViewModel();
			ApplicationService.Run(multiclientViewModel, true, true);

			if (!File.Exists(AppDataFolderHelper.GetMulticlientFile()))
			{
				MessageBoxService.ShowError("Не найден файл конфигурации. Приложение будет закрыто");
				Shutdown();
				return;
			}

			var passwordViewModel = new PasswordViewModel();
			if (DialogService.ShowModalWindow(passwordViewModel))
			{
				if (passwordViewModel.MulticlientConfiguration == null && passwordViewModel.MulticlientConfiguration.MulticlientDatas.Count == 0)
				{
					MessageBoxService.ShowError("Конфигурация пуста. Приложение будет закрыто");
					Shutdown();
					return;
				}
				multiclientViewModel.Initialize(passwordViewModel.MulticlientConfiguration);
			}
			else
			{
				Shutdown();
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			Environment.Exit(0);
		}
	}
}