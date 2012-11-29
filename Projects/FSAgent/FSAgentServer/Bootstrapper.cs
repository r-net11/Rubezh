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

namespace FSAgentServer
{
	public static class Bootstrapper
	{
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;
		static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);
		static WatcherManager WatcherManager;

		public static void Run()
		{
			try
			{
				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(BalloonToolTipViewModel).Assembly, "BalloonTrayTip/DataTemplates/Dictionary.xaml"));

				WindowThread = new Thread(new ThreadStart(OnWorkThread));
				WindowThread.Priority = ThreadPriority.Highest;
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();
				MainViewStartedEvent.WaitOne();

				UILogger.Log("Открытие хоста");
				FSAgentServiceHost.Start();
				FSAgentLoadHelper.NotifyStartCompleted();
				WatcherManager = new WatcherManager();
				WatcherManager.Start();
				UILogger.Log("Готово");
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера", true);
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
			MainViewStartedEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			if (WindowThread != null)
			{
				WindowThread.Interrupt();
				WindowThread = null;
			}

			System.Environment.Exit(1);
		}
	}
}