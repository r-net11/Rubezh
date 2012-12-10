using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip.ViewModels;
using Infrastructure.Common.Windows;
using FSAgentServer.ViewModels;
using Common;
using Infrastructure.Common.BalloonTrayTip;
using System.Windows.Threading;

namespace FSAgentServer
{
	public static class Bootstrapper
	{
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;
		public static AutoResetEvent BootstrapperLoadEvent = new AutoResetEvent(false);
		static WatcherManager WatcherManager;

		public static void Run()
		{
			try
			{
                BalloonHelper.Initialize();
                var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
				
                WindowThread = new Thread(new ThreadStart(OnWorkThread));
				WindowThread.Priority = ThreadPriority.Highest;
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();
                if (!BootstrapperLoadEvent.WaitOne(TimeSpan.FromMinutes(5)))
                {
                    BalloonHelper.Show("Агент Firesec", "Ошибка во время загрузки. Истекло время ожидания загрузки окна");
                }
                BootstrapperLoadEvent = new AutoResetEvent(false);
                
				UILogger.Log("Открытие хоста");
				FSAgentServiceHost.Start();
				UILogger.Log("Соединение с драйвером");
				WatcherManager = new WatcherManager();
				WatcherManager.Start();
                
                if (!BootstrapperLoadEvent.WaitOne(TimeSpan.FromMinutes(5)))
                {
                    BalloonHelper.Show("Агент Firesec", "Ошибка во время загрузки. Истекло время ожидания загрузки драйверов");
                    UILogger.Log("Ошибка во время загрузки. Истекло время ожидания загрузки драйверов");
                }
				UILogger.Log("Готово");
                FSAgentLoadHelper.SetStatus(FSAgentState.Opened);
            }
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера");
				Close();
			}
		}

		static void OnWorkThread()
		{
			try
			{
				MainViewModel = new MainViewModel();
				ApplicationService.Run(MainViewModel);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Bootstrapper.OnWorkThread");
			}
			BootstrapperLoadEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
            FSAgentLoadHelper.SetStatus(FSAgentState.Closed);
            if (WatcherManager.Current != null)
            {
                WatcherManager.Current.Stop();
            }
			if (WindowThread != null)
			{
				WindowThread.Interrupt();
				WindowThread = null;
			}
			System.Environment.Exit(1);
		}
	}
}