using System.Threading;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Microsoft.Practices.Prism.Events;

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
					MessageBoxService.ShowError("Ошибка соединения с сервером: " + message);
					return;
				}
			}

			ServiceFactoryBase.Events = new EventAggregator();
			Watcher.MustShowProgress = false;
			GKDBHelper.CanAdd = false;

			FiresecManager.GetConfiguration("GKSDK/Configuration");
			GKDriversCreator.Create();
			XManager.UpdateConfiguration();
			XManager.CreateStates();
			DatabaseManager.Convert();
			WatcherManager.Start();
		}
	}
}