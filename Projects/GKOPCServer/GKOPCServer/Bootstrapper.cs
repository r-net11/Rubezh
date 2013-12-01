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

			if (!GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
			{
				GKProcessorManager.GKCallbackResultEvent -= new Action<GKCallbackResult>(OnGKCallbackResult);
				GKProcessorManager.GKCallbackResultEvent += new Action<GKCallbackResult>(OnGKCallbackResult);
			}
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
				if (gkCallbackResult.DeviceStates.Count > 0)
				{
					foreach (var remoteDeviceState in gkCallbackResult.DeviceStates)
					{
						var device = XManager.Devices.FirstOrDefault(x => x.UID == remoteDeviceState.UID);
						if (device != null)
						{
							device.DeviceState.StateClasses = remoteDeviceState.StateClasses;
							device.DeviceState.StateClass = remoteDeviceState.StateClass;
							device.DeviceState.OnDelay = remoteDeviceState.OnDelay;
							device.DeviceState.HoldDelay = remoteDeviceState.HoldDelay;
							device.DeviceState.OffDelay = remoteDeviceState.OffDelay;
							device.DeviceState.OnStateChanged();
						}
					}
				}
				if (gkCallbackResult.ZoneStates.Count > 0)
				{
					foreach (var remoteZoneState in gkCallbackResult.ZoneStates)
					{
						var zone = XManager.Zones.FirstOrDefault(x => x.UID == remoteZoneState.UID);
						if (zone != null)
						{
							zone.ZoneState.StateClasses = remoteZoneState.StateClasses;
							zone.ZoneState.StateClass = remoteZoneState.StateClass;
							zone.ZoneState.OnDelay = remoteZoneState.OnDelay;
							zone.ZoneState.HoldDelay = remoteZoneState.HoldDelay;
							zone.ZoneState.OffDelay = remoteZoneState.OffDelay;
							zone.ZoneState.OnStateChanged();
						}
					}
				}
				if (gkCallbackResult.DirectionStates.Count > 0)
				{
					foreach (var remoteDirectionState in gkCallbackResult.DirectionStates)
					{
						var direction = XManager.Directions.FirstOrDefault(x => x.UID == remoteDirectionState.UID);
						if (direction != null)
						{
							direction.DirectionState.StateClasses = remoteDirectionState.StateClasses;
							direction.DirectionState.StateClass = remoteDirectionState.StateClass;
							direction.DirectionState.OnDelay = remoteDirectionState.OnDelay;
							direction.DirectionState.HoldDelay = remoteDirectionState.HoldDelay;
							direction.DirectionState.OffDelay = remoteDirectionState.OffDelay;
							direction.DirectionState.OnStateChanged();
						}
					}
				}
			});
		}
	}
}