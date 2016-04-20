using System;
using System.IO;
using System.Windows;
using FireMonitor.Multiclient.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Theme;

namespace FireMonitor.Multiclient
{
	public partial class App : Application
	{
		const string SignalId = "{8D414CAF-EA66-4904-BD6A-0EC72802E2C8}";
		const string WaitId = "{BF9B5F62-C57A-49BC-9BC1-245734665A22}";

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			using (new DoubleLaunchLocker(SignalId, WaitId, true, true))
			{
				ServiceFactory.Initialize();

				var folderName = AppDataFolderHelper.GetFolder("Multiclient/Configuration");
				if (Directory.Exists(folderName))
					Directory.Delete(folderName, true);
				Directory.CreateDirectory(folderName);

				ThemeHelper.LoadThemeFromRegister();

				if (!LicenseHelper.CheckLicense(true))
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
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			Environment.Exit(0);
		}
	}
}