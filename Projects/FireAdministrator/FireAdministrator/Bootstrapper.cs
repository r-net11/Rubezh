using Common;
using FireAdministrator.ViewModels;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
//using Infrastructure.Common.Services.Configuration;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Services;
using System;
using System.Windows;

namespace FireAdministrator
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
			LoadingErrorManager.Clear();
			ServiceFactory.Initialize(
				new LayoutService(),
				new ValidationService()
				//new UiElementsVisibilityService(),
				//new ConfigurationElementsAvailabilityService()
				);
			var assembly = GetType().Assembly;
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(assembly, "DataTemplates/Dictionary.xaml"));
			ServiceFactory.StartupService.Show();
			if (ServiceFactory.StartupService.PerformLogin())
			{
				try
				{
					// При получении от сервера команды на разрыв соединения выводим соответствующее предупреждение и завершаем работу
					SafeFiresecService.DisconnectClientCommandEvent += () =>
					{
						ApplicationService.Invoke(() => MessageBoxService.ShowWarning("Соединение было разорвано Сервером.\nРабота приложения будет завершена."));
						ApplicationService.ShutDown();
					};

					// При получении от сервера уведомления о смене лицензии выводим соответствующее предупреждение и завершаем работу
					//SafeFiresecService.LicenseChangedEvent += () =>
					//{
					//	ApplicationService.Invoke(() => MessageBoxService.ShowWarning("Соединение было разорвано Сервером в связи с изменением лицензии.\nРабота приложения будет завершена."));
					//	ApplicationService.ShutDown();
					//};

					// Получаем данные лицензии с Сервера и инициализируем зависимые от них службы
				//	var licenseData = FiresecManager.FiresecService.GetLicenseData().Result;
					//ServiceFactory.UiElementsVisibilityService.Initialize(licenseData);
				//	ServiceFactory.ConfigurationElementsAvailabilityService.Initialize(licenseData);

					ServiceFactory.StartupService.ShowLoading("Загрузка модулей", 5);
					CreateModules();

					ServiceFactory.StartupService.DoStep("Чтение конфигурации");
					ServiceFactory.StartupService.AddCount(GetModuleCount() + 6);

					ServiceFactory.StartupService.DoStep("Синхронизация файлов");
					FiresecManager.UpdateFiles();

					ServiceFactory.StartupService.DoStep("Загрузка конфигурации с сервера");
					FiresecManager.GetConfiguration("Administrator/Configuration");

					BeforeInitialize(true);

					ServiceFactory.StartupService.DoStep("Загрузка клиентских настроек");
					ClientSettings.LoadSettings();

					ServiceFactory.StartupService.DoStep("Проверка прав пользователя");
					if (FiresecManager.CheckPermission(PermissionType.Adm_ViewConfig) == false) //TODO: Переместить конструкцию выше. Допускается использование сервера до проверки прав пользователя
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						FiresecManager.Disconnect();
					}
					else if (Application.Current != null)
					{
						var shell = new AdministratorShellViewModel();
						shell.LogoSource = "Logo";
						ServiceFactory.MenuService = new MenuService((vm) => ((MenuViewModel)shell.Toolbar).ExtendedMenu = vm);
						RunShell(shell);
					}
					ServiceFactory.StartupService.Close();

					AterInitialize();
					FiresecManager.StartPoll();

					SafeFiresecService.SKDProgressCallbackEvent -= new Action<FiresecAPI.SKDProgressCallback>(OnSKDProgressCallbackEvent);
					SafeFiresecService.SKDProgressCallbackEvent += new Action<FiresecAPI.SKDProgressCallback>(OnSKDProgressCallbackEvent);

					ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Subscribe(OnConfigurationChanged);
					ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Subscribe(OnConfigurationClosed);
					MutexHelper.KeepAlive();
				}
				catch (StartupCancellationException)
				{
					throw;
				}
				catch (Exception e)
				{
					Logger.Error(e, "Bootstrapper.Initialize");
					MessageBoxService.ShowException(e);
					if (Application.Current != null)
						Application.Current.Shutdown();
					return;
				}
			}
			else
			{
				if (Application.Current != null)
					Application.Current.Shutdown();
				return;
			}
		}

		void OnSKDProgressCallbackEvent(SKDProgressCallback SKDProgressCallback)
		{
			ApplicationService.Invoke(() =>
			{
				switch (SKDProgressCallback.SKDProgressCallbackType)
				{
					case SKDProgressCallbackType.Start:
						if (SKDProgressCallback.SKDProgressClientType == SKDProgressClientType.Administrator)
						{
							LoadingService.Show(SKDProgressCallback.Title, SKDProgressCallback.Text, SKDProgressCallback.StepCount, SKDProgressCallback.CanCancel);
						}
						return;

					case SKDProgressCallbackType.Progress:
						if (SKDProgressCallback.SKDProgressClientType == SKDProgressClientType.Administrator)
						{
							//LoadingService.DoStep(SKDProgressCallback.Text, SKDProgressCallback.Title, SKDProgressCallback.StepCount, SKDProgressCallback.CurrentStep, SKDProgressCallback.CanCancel);
							LoadingService.DoStep(SKDProgressCallback.Text);
							if (LoadingService.IsCanceled)
							{
								FiresecManager.FiresecService.CancelSKDProgress(SKDProgressCallback.UID, FiresecManager.CurrentUser.Name);
							}
						}
						return;

					case SKDProgressCallbackType.Stop:
						LoadingService.Close();
						return;
				}
			});
		}

		private void OnConfigurationChanged(object obj)
		{
			LoadingErrorManager.Clear();
			ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
			ServiceFactory.ContentService.Clear();
			InitializeModules();
			LoadingService.Close();
		}
		private void OnConfigurationClosed(object obj)
		{
			ServiceFactory.ContentService.Close();
		}

		private void CloseOnException(string message)
		{
			MessageBoxService.ShowError(message);
			if (Application.Current != null)
				Application.Current.Shutdown();
		}
	}
}