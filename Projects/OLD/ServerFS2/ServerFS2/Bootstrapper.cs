using System;
using System.Diagnostics;
using System.Threading;
using Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using ServerFS2.Processor;
using ServerFS2.Service;
using ServerFS2.ViewModels;

namespace ServerFS2
{
	public static class Bootstrapper
	{
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;

		public static void Run(bool loadConfiguation = true)
		{
			try
			{
				if (loadConfiguation)
				{
					ConfigurationManager.Load();
				}
				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

				WindowThread = new Thread(new ThreadStart(OnWorkThread));
				WindowThread.Priority = ThreadPriority.Highest;
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();

				UILogger.Log("Открытие хоста");
				FS2ServiceHost.Start();
				UILogger.Log("Запуск мониторинга");
				MainManager.StartMonitoring();

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
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			FSAgentLoadHelper.SetStatus(FSAgentState.Closed);
			MainManager.StopMonitoring();
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