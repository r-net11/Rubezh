﻿using System;
using System.Windows;
using Common;
using FireAdministrator.ViewModels;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Services;
using RubezhAPI.Automation;
using Infrastructure.Automation;

namespace FireAdministrator
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
			LoadingErrorManager.Clear();
			ServiceFactory.Initialize(new LayoutService(), new ValidationService());
			var assembly = GetType().Assembly;
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(assembly, "DataTemplates/Dictionary.xaml"));
			ServiceFactory.StartupService.Show();
			if (ServiceFactory.StartupService.PerformLogin())
			{
				try
				{
					ServiceFactory.StartupService.DoStep("Загрузка лицензии");
					ClientManager.GetLicense();

					ServiceFactory.StartupService.ShowLoading("Загрузка модулей", 5);
					CreateModules();

					ServiceFactory.StartupService.DoStep("Чтение конфигурации");
					ServiceFactory.StartupService.AddCount(GetModuleCount() + 6);

					ServiceFactory.StartupService.DoStep("Синхронизация файлов");
					ClientManager.UpdateFiles();

					ServiceFactory.StartupService.DoStep("Загрузка конфигурации с сервера");
					ClientManager.GetConfiguration("Administrator/Configuration");
					ProcedureExecutionContext.Initialize(
						ContextType.Client,
						ClientManager.SystemConfiguration,
						ClientManager.SecurityConfiguration
						);

					GKDriversCreator.Create();
					BeforeInitialize(true);

					ServiceFactory.StartupService.DoStep("Проверка прав пользователя");
					if (ClientManager.CheckPermission(PermissionType.Adm_ViewConfig) == false)
					{
						MessageBoxService.Show("Нет прав на работу с программой");
						ClientManager.Disconnect();
						if (Application.Current != null)
							Application.Current.Shutdown();
						return;
					}
					else if (Application.Current != null)
					{
						var shell = new AdministratorShellViewModel();
						shell.LogoSource = "rubezhLogo";
						ServiceFactory.MenuService = new MenuService((vm) => ((MenuViewModel)shell.Toolbar).ExtendedMenu = vm);
						RunShell(shell);
					}
					ServiceFactory.StartupService.Close();

					AterInitialize();
					ClientManager.StartPoll();

					SafeFiresecService.GKProgressCallbackEvent -= new Action<RubezhAPI.GKProgressCallback>(OnGKProgressCallbackEvent);
					SafeFiresecService.GKProgressCallbackEvent += new Action<RubezhAPI.GKProgressCallback>(OnGKProgressCallbackEvent);

					ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Subscribe(OnConfigurationChanged);
					ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Subscribe(OnConfigurationClosed);

					SafeFiresecService.ConfigurationChangedEvent += () => { ApplicationService.Invoke(OnConfigurationChanged); };
					
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

		void OnConfigurationChanged()
		{
			ClientManager.GetLicense();
			ProcedureExecutionContext.UpdateConfiguration(ClientManager.SystemConfiguration, ClientManager.SecurityConfiguration);
		}
		void OnGKProgressCallbackEvent(GKProgressCallback gkProgressCallback)
		{
			ApplicationService.Invoke(() =>
			{
				switch (gkProgressCallback.GKProgressCallbackType)
				{
					case GKProgressCallbackType.Start:
						if (gkProgressCallback.GKProgressClientType == GKProgressClientType.Administrator)
						{
							LoadingService.Show(gkProgressCallback.Title, gkProgressCallback.Text, gkProgressCallback.StepCount, gkProgressCallback.CanCancel);
						}
						return;

					case GKProgressCallbackType.Progress:
						if (gkProgressCallback.GKProgressClientType == GKProgressClientType.Administrator)
						{
							LoadingService.DoStep(gkProgressCallback.Text, gkProgressCallback.Title, gkProgressCallback.StepCount, gkProgressCallback.CurrentStep, gkProgressCallback.CanCancel);
							if (LoadingService.IsCanceled)
							{
								ClientManager.FiresecService.CancelGKProgress(gkProgressCallback.UID, ClientManager.CurrentUser.Name);
							}
						}
						return;

					case GKProgressCallbackType.Stop:
						LoadingService.Close();
						return;
				}
			});
		}

		private void OnConfigurationChanged(object obj)
		{
			LoadingErrorManager.Clear();
			ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
			ServiceFactory.ContentService.Invalidate();
			ProcedureExecutionContext.UpdateConfiguration(ClientManager.SystemConfiguration, ClientManager.SecurityConfiguration);
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