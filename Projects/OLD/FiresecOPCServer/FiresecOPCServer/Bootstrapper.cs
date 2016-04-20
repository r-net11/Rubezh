using System;
using System.Threading;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using FiresecOPCServer.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;

namespace FiresecOPCServer
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
			WindowThread.Name = "OPC Main Window";
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

			InitializeFs();
			FiresecOPCManager.Start();
			UILogger.Log("Готово");
		}

		static void InitializeFs()
		{
			UILogger.Log("Загрузка конфигурации с сервера");
			FiresecManager.GetConfiguration("OPC/Configuration");
			UILogger.Log("Загрузка драйвера устройств");
			FiresecManager.InitializeFiresecDriver(true);
			UILogger.Log("Синхронизация конфигурации");
			FiresecManager.FiresecDriver.Synchronyze();
			UILogger.Log("Старт мониторинга");
			FiresecManager.FiresecDriver.StartWatcher(true, false);
			FiresecManager.FSAgent.Start();
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