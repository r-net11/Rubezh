using Common;
using FireAdministrator.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Client;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure.Services;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Linq;
using System.Windows;

namespace FireAdministrator
{
	public class Bootstrapper : BaseBootstrapper
	{
		public void Initialize()
		{
			LoadingErrorManager.Clear();
			ServiceFactory.Initialize(new LayoutService(), new ValidationService());
			var assembly = GetType().Assembly;
			ServiceFactory.ResourceService.AddResource(assembly, "DataTemplates/Dictionary.xaml");
			ServiceFactory.StartupService.Show();
			if (ServiceFactory.StartupService.PerformLogin())
			{
				Login = ServiceFactory.StartupService.Login;
				Password = ServiceFactory.StartupService.Password;
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
					ClientManager.GetConfiguration("Administrator\\Configuration");
					ProcedureExecutionContext.Initialize(
						ContextType.Client,
						() => { return ClientManager.SystemConfiguration; }
						);

					ObjectReference.ResolveObjectName += ObjectReference_ResolveObjectName;
					ObjectReference.ResolveObjectValue += ObjectReference_ResolveObjectValue;

					GKDriversCreator.Create();
					BeforeInitialize(true);

					if (Application.Current != null)
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
					SafeFiresecService.RestartEvent += () => { ApplicationService.Invoke(Restart); };

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

		object ObjectReference_ResolveObjectValue(Guid objectUID, ObjectType objectType)
		{
			throw new NotImplementedException();
		}

		string ObjectReference_ResolveObjectName(Guid objectUID, ObjectType objectType)
		{
			if (objectUID == Guid.Empty)
				return "Null";
			switch (objectType)
			{
				case ObjectType.Device:
					var device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objectUID);
					if (device != null)
						return device.PresentationName;
					break;
				case ObjectType.Zone:
					var zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objectUID);
					if (zone != null)
						return zone.PresentationName;
					break;
				case ObjectType.Direction:
					var direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objectUID);
					if (direction != null)
						return direction.PresentationName;
					break;
				case ObjectType.Delay:
					var delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == objectUID);
					if (delay != null)
						return delay.PresentationName;
					break;
				case ObjectType.GuardZone:
					var guardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUID);
					if (guardZone != null)
						return guardZone.PresentationName;
					break;
				case ObjectType.PumpStation:
					var pumpStation = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == objectUID);
					if (pumpStation != null)
						return pumpStation.PresentationName;
					break;
				case ObjectType.MPT:
					var mpt = GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == objectUID);
					if (mpt != null)
						return mpt.PresentationName;
					break;
				case ObjectType.VideoDevice:
					var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == objectUID);
					if (camera != null)
						return camera.PresentationName;
					break;
				case ObjectType.GKDoor:
					var gKDoor = GKManager.Doors.FirstOrDefault(x => x.UID == objectUID);
					if (gKDoor != null)
						return gKDoor.PresentationName;
					break;
				case ObjectType.Organisation:
					var organisation = RubezhClient.SKDHelpers.OrganisationHelper.GetSingle(objectUID);
					if (organisation != null)
						return organisation.Name;
					break;
			}
			return "Null";
		}

		void OnConfigurationChanged()
		{
			ClientManager.GetLicense();
		}
		void Restart()
		{
			Restart(Login, Password);
		}
		public void Restart(string login = null, string password = null)
		{
			using (new WaitWrapper())
			{
				ApplicationService.ApplicationWindow.IsEnabled = false;
				ServiceFactory.ContentService.Invalidate();
				ClientManager.FiresecService.StopPoll();
				LoadingErrorManager.Clear();
				ApplicationService.CloseAllWindows();
				ServiceFactory.Layout.Close();
				ApplicationService.ShutDown();
			}
			if (login != null && password != null)
			{
				Login = login;
				Password = password;
			}
			RestartApplication();
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