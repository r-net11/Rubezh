using System;
using System.Diagnostics;
using System.Threading;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;
using ServerFS2.Service;
using ServerFS2.ViewModels;

namespace ServerFS2
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
				ConfigurationManager.Load();
				USBManager.Initialize();
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
					BalloonHelper.ShowFromAgent("Ошибка во время загрузки. Истекло время ожидания загрузки окна");
				}
				BootstrapperLoadEvent = new AutoResetEvent(false);

				UILogger.Log("Открытие хоста");
				FS2ServiceHost.Start();
				UILogger.Log("Соединение с драйвером");
				WatcherManager = new WatcherManager();
				WatcherManager.Start();

				if (!BootstrapperLoadEvent.WaitOne(TimeSpan.FromMinutes(5)))
				{
					BalloonHelper.ShowFromAgent("Ошибка во время загрузки. Истекло время ожидания загрузки драйверов");
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

		private static void OnWorkThread()
		{
			try
			{
				MainViewModel = new MainViewModel();
				ApplicationService.Run(MainViewModel, false, false);
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

#if DEBUG
			return;
#endif
			Process.GetCurrentProcess().Kill();
		}
	}
}