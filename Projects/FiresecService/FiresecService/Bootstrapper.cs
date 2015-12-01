using Common;
using FiresecAPI;
using FiresecService.Report;
using FiresecService.Service;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace FiresecService
{
	public static class Bootstrapper
	{
		private static Thread WindowThread = null;
		private static MainViewModel MainViewModel;
		private static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

		public static void Run()
		{
			try
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Logger.Trace(SystemInfo.GetString());
				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
				WindowThread = new Thread(OnWorkThread) {Name = "Main window", Priority = ThreadPriority.Highest};
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();
				MainViewStartedEvent.WaitOne();

				UILogger.Log("Загрузка конфигурации");

				ConfigurationCashHelper.Update();

				ServiceHealthStatus databaseServiceHealthStatus;
				try
				{
					PatchManager.Patch();
					databaseServiceHealthStatus = ServiceHealthStatus.Alive;
				}
				catch (Exception)
				{
					databaseServiceHealthStatus = ServiceHealthStatus.Dead;
					MessageBox.Show("Не удалось подключиться к базе данных");
					Application.Current.MainWindow.Close();
				}

				UILogger.Log("Открытие хоста");
				try
				{
					FiresecServiceManager.Open();
				}
				catch (Exception)
				{
					MessageBox.Show("При открытии хоста обнаружена ошибка");
					Application.Current.MainWindow.Close();
				}

				ServerLoadHelper.SetStatus(FSServerState.Opened);
				UILogger.Log("Создание конфигурации СКД");
				try
				{
					SKDProcessor.Start();
				}
				catch (Exception)
				{
					MessageBox.Show("В конфигурационном файле SKD содержиться ошибка или он отсутствует");
					Application.Current.MainWindow.Close();
				}

				ServiceHealthStatus reportServiceHealthStatus;
				UILogger.Log("Запуск сервиса отчетов");
				ReportServiceManager.Run();
				if (ReportServiceManager.IsRunning)
				{
					reportServiceHealthStatus = ServiceHealthStatus.Alive;
					UILogger.Log("Сервис отчетов запущен" + ReportServiceManager.Address);
					ReportServiceManager.Addresses.ForEach(UILogger.Log);
				}
				else
				{
					reportServiceHealthStatus = ServiceHealthStatus.Dead;
					UILogger.Log("[*] Сервис отчетов не запущен");
				}

				ServiceHealthStatus automationServiceHealthStatus;
				UILogger.Log("Запуск автоматизации");
				try
				{
					ScheduleRunner.Start();
					automationServiceHealthStatus = ServiceHealthStatus.Alive;
				}
				catch (Exception)
				{
					automationServiceHealthStatus = ServiceHealthStatus.Dead;
					UILogger.Log("[*] Автоматизация не запущена");
				}

				FiresecServiceManager.SafeFiresecService.FiresecService.DatabaseServiceHealthStatus = databaseServiceHealthStatus;
				FiresecServiceManager.SafeFiresecService.FiresecService.ReportServiceHealthStatus = reportServiceHealthStatus;
				FiresecServiceManager.SafeFiresecService.FiresecService.AutomationServiceHealthStatus = automationServiceHealthStatus;

				UILogger.Log("Готово");
				ProcedureRunner.RunOnServerRun();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера");
				BalloonHelper.ShowFromServer("Ошибка во время загрузки");
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
				Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");

				BalloonHelper.ShowFromServer("Ошибка во время загрузки");
			}
			MainViewStartedEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			ServerLoadHelper.SetStatus(FSServerState.Closed);
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