using System;
using System.Configuration;
using System.Windows;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Services;
using Common.GK;
using GKProcessor;
using Microsoft.Practices.Prism.Events;
using FiresecClient;
using FiresecAPI.Models;
using System.Threading;

namespace GKSDK
{
	public class MainViewModel : BaseViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }
		public ZonesViewModel ZonesViewModel { get; private set; }
		public JournalsViewModel JournalsViewModel { get; private set; }

		public MainViewModel()
		{
			InitializeGK();

			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			JournalsViewModel = new JournalsViewModel();
		}

		static void InitializeGK()
		{
			for (int i = 1; i <= 10; i++)
			{
				var message = FiresecManager.Connect(ClientType.Other, ConnectionSettingsManager.ServerAddress, GlobalSettingsHelper.GlobalSettings.Login, GlobalSettingsHelper.GlobalSettings.Password);
				if (message == null)
					break;
				Thread.Sleep(5000);
				if (i == 10)
				{
					//UILogger.Log("Ошибка соединения с сервером: " + message);
					return;
				}
			}

			ServiceFactoryBase.Events = new EventAggregator();
			Watcher.MustShowProgress = false;
			GKDBHelper.CanAdd = false;

			//UILogger.Log("Загрузка конфигурации с сервера");
			FiresecManager.GetConfiguration("GKSDK/Configuration");

			//UILogger.Log("Создание драйверов");
			GKDriversCreator.Create();

			//UILogger.Log("Обновление конфигурации");
			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DatabaseManager.Convert();

			//UILogger.Log("Старт мониторинга");
			WatcherManager.Start();
		}
	}
}