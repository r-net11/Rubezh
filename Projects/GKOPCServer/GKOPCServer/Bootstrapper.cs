using System;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using GKOPCServer.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using GKProcessor;
using Infrastructure.Common.Services;
using Microsoft.Practices.Prism.Events;
using XFiresecAPI;

namespace GKOPCServer
{
	public static class Bootstrapper
	{
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;
		static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

		public static void Run()
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

			WindowThread = new Thread(new ThreadStart(OnWorkThread));
			WindowThread.Priority = ThreadPriority.Highest;
			WindowThread.SetApartmentState(ApartmentState.STA);
			WindowThread.IsBackground = true;
			WindowThread.Start();
			MainViewStartedEvent.WaitOne();

			UILogger.Log("Соединение с сервером");
			for (int i = 1; i <= 10; i++)
			{
				var message = FiresecManager.Connect(ClientType.OPC, ConnectionSettingsManager.ServerAddress, GlobalSettingsHelper.GlobalSettings.Login, GlobalSettingsHelper.GlobalSettings.Password);
				if (message == null)
					break;
				Thread.Sleep(5000);
				if (i == 10)
				{
					UILogger.Log("Ошибка соединения с сервером: " + message);
					return;
				}
			}

			InitializeGK();
			GKOPCManager.Start();
			UILogger.Log("Готово");
		}

		static void InitializeGK()
		{
			ServiceFactoryBase.Events = new EventAggregator();
			GKDBHelper.CanAdd = false;

			UILogger.Log("Загрузка конфигурации с сервера");
			FiresecManager.GetConfiguration("GKOPC/Configuration");

			UILogger.Log("Создание драйверов");
			GKDriversCreator.Create();

			UILogger.Log("Обновление конфигурации");
			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DescriptorsManager.Create();

			UILogger.Log("Старт мониторинга");
			WatcherManager.Start();

			SafeFiresecService.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			SafeFiresecService.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);

			GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
			GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);
		}

		static void OnWorkThread()
		{
			try
			{
				MainViewModel = new MainViewModel();
				ApplicationService.Run(MainViewModel, false, false);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");
			}
			MainViewStartedEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			WatcherManager.Stop();
			FiresecManager.Disconnect();
			if (WindowThread != null)
			{
				WindowThread.Interrupt();
				WindowThread = null;
			}

			System.Environment.Exit(1);
		}

		static void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			ApplicationService.Invoke(() =>
			{
				foreach (var remoteDeviceState in gkCallbackResult.DeviceStates)
				{
					var device = XManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
					if (device != null)
					{
						device.State.StateClasses = remoteDeviceState.StateClasses;
						device.State.StateClass = remoteDeviceState.StateClass;
						device.State.OnDelay = remoteDeviceState.OnDelay;
						device.State.HoldDelay = remoteDeviceState.HoldDelay;
						device.State.OffDelay = remoteDeviceState.OffDelay;
						device.State.OnStateChanged();
					}
				}
				foreach (var remoteZoneState in gkCallbackResult.ZoneStates)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
					if (zone != null)
					{
						zone.State.StateClasses = remoteZoneState.StateClasses;
						zone.State.StateClass = remoteZoneState.StateClass;
						zone.State.OnDelay = remoteZoneState.OnDelay;
						zone.State.HoldDelay = remoteZoneState.HoldDelay;
						zone.State.OffDelay = remoteZoneState.OffDelay;
						zone.State.OnStateChanged();
					}
				}
				foreach (var remoteDirectionState in gkCallbackResult.DirectionStates)
				{
					var direction = XManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
					if (direction != null)
					{
						direction.State.StateClasses = remoteDirectionState.StateClasses;
						direction.State.StateClass = remoteDirectionState.StateClass;
						direction.State.OnDelay = remoteDirectionState.OnDelay;
						direction.State.HoldDelay = remoteDirectionState.HoldDelay;
						direction.State.OffDelay = remoteDirectionState.OffDelay;
						direction.State.OnStateChanged();
					}
				}
			});
		}
	}
}