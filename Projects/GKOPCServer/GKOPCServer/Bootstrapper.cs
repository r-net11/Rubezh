using System;
using System.Threading;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using GKOPCServer.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using GKProcessor;
using Common.GK;
using Infrastructure.Common.Services;
using Microsoft.Practices.Prism.Events;

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
			Watcher.MustShowProgress = false;
			GKDBHelper.CanAdd = false;

			UILogger.Log("Загрузка конфигурации с сервера");
			FiresecManager.GetConfiguration("GKOPC/Configuration");

			UILogger.Log("Создание драйверов");
			GKDriversCreator.Create();

			UILogger.Log("Обновление конфигурации");
			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DatabaseManager.Convert();

			UILogger.Log("Старт мониторинга");
			WatcherManager.Start();
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
	}
}