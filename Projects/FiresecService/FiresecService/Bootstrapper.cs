using Common;
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
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;
		static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

		public static void Run()
		{
			try
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Logger.Trace(SystemInfo.GetString());
				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
				WindowThread = new Thread(new ThreadStart(OnWorkThread));
				WindowThread.Name = "Main window";
				WindowThread.Priority = ThreadPriority.Highest;
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();
				MainViewStartedEvent.WaitOne();

				UILogger.Log("Загрузка конфигурации");

				ConfigurationCashHelper.Update();

				try
				{
					PatchManager.Patch();
				}
				catch (Exception)
				{

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

				UILogger.Log("Запуск сервиса отчетов");
				ReportServiceManager.Run();
				UILogger.Log("Сервис отчетов запущен: " + ConnectionSettingsManager.ReportServerAddress);

				UILogger.Log("Запуск автоматизации");
				ScheduleRunner.Start();

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