using System.Threading;
using Common;
using FireAdministrator.ViewModels;
using Infrastructure.Common.Services;
using Localization.FireAdministrator.Common;
using StrazhAPI;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Services.Configuration;
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
				new ValidationService(),
				new UiElementsVisibilityService(),
				new ConfigurationElementsAvailabilityService());
			var assembly = GetType().Assembly;
			ServiceFactoryBase.ResourceService.AddResource(new ResourceDescription(assembly, "DataTemplates/Dictionary.xaml"));
			ServiceFactory.StartupService.Show();
			if (ServiceFactory.StartupService.PerformLogin())
			{
				try
				{
					// При получении от сервера команды на разрыв соединения выводим соответствующее предупреждение и завершаем работу
					SafeFiresecService.DisconnectClientCommandEvent += showNotification =>
					{
						if (showNotification)
                            ApplicationService.Invoke(() => MessageBoxService.ShowWarning(CommonResources.ServerConnectionRefused));
						ApplicationService.ShutDown();
					};

					// При получении от сервера уведомления о смене лицензии выводим соответствующее предупреждение и завершаем работу
					SafeFiresecService.LicenseChangedEvent += () =>
					{
                        ApplicationService.Invoke(() => MessageBoxService.ShowWarning(CommonResources.ServerConnectionRefused_License));
						ApplicationService.ShutDown();
					};

					// Получаем данные лицензии с Сервера и инициализируем зависимые от них службы
					var licenseData = FiresecManager.FiresecService.GetLicenseData().Result;
					ServiceFactory.UiElementsVisibilityService.Initialize(licenseData);
					ServiceFactory.ConfigurationElementsAvailabilityService.Initialize(licenseData);

                    ServiceFactory.StartupService.ShowLoading(CommonResources.ModulesLoad, 5);
					CreateModules();

                    ServiceFactory.StartupService.DoStep(CommonResources.ReadingConfig);
					ServiceFactory.StartupService.AddCount(GetModuleCount() + 6);

                    ServiceFactory.StartupService.DoStep(CommonResources.FileSynchronisation);
					FiresecManager.UpdateFiles();

                    ServiceFactory.StartupService.DoStep(CommonResources.ServerConfigLoad);
					FiresecManager.GetConfiguration("Administrator/Configuration");

					BeforeInitialize(true);

                    ServiceFactory.StartupService.DoStep(CommonResources.ClientSettingsLoad);
					ClientSettings.LoadSettings();

                    ServiceFactory.StartupService.DoStep(CommonResources.CheckUserPermission);
					if (FiresecManager.CheckPermission(PermissionType.Adm_ViewConfig) == false) //TODO: Переместить конструкцию выше. Допускается использование сервера до проверки прав пользователя
					{
                        MessageBoxService.Show(CommonResources.NoWorkPermissions);
						FiresecManager.Disconnect();
					}
					else if (Application.Current != null)
					{
						var shell = new AdministratorShellViewModel {LogoSource = "Logo"};
						ServiceFactory.MenuService = new MenuService(vm => ((MenuViewModel)shell.Toolbar).ExtendedMenu = vm);
						RunShell(shell);
					}
					ServiceFactory.StartupService.Close();

					AterInitialize();
					FiresecManager.StartPoll();

					SafeFiresecService.SKDProgressCallbackEvent -= OnSKDProgressCallbackEvent;
					SafeFiresecService.SKDProgressCallbackEvent += OnSKDProgressCallbackEvent;

					ServiceFactoryBase.Events.GetEvent<ConfigurationChangedEvent>().Subscribe(OnConfigurationChanged);
					ServiceFactoryBase.Events.GetEvent<ConfigurationClosedEvent>().Subscribe(OnConfigurationClosed);
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
				}
			}
			else
			{
				if (Application.Current != null)
					Application.Current.Shutdown();
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
			ServiceFactoryBase.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
			ServiceFactoryBase.ContentService.Clear();
			InitializeModules();
			LoadingService.Close();
		}
		private static void OnConfigurationClosed(object obj)
		{
			ServiceFactoryBase.ContentService.Close();
		}

		private void CloseOnException(string message)
		{
			MessageBoxService.ShowError(message);
			if (Application.Current != null)
				Application.Current.Shutdown();
		}
	}
}