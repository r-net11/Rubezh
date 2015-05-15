using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Common;
using FiresecService.Report;
using FiresecService.Service;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

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
				//EntLibTest();
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
				UILogger.Log("Создание конфигурации ГК");
				GKProcessor.Create();
				PatchManager.Patch();
				UILogger.Log("Открытие хоста");
				FiresecServiceManager.Open();
				ServerLoadHelper.SetStatus(FSServerState.Opened);
				UILogger.Log("Запуск ГК");
				GKProcessor.Start();
				UILogger.Log("Создание конфигурации СКД");
				SKDProcessor.Start();

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

		static void EntLibTest()
		{
			string myConnectionString = @"Database=PassJournal_1;Server=(local)\SQLEXPRESS;Integrated Security=True;Language='English'";
			SqlDatabase sqlDatabase = new SqlDatabase(myConnectionString);
			using (IDataReader reader = sqlDatabase.ExecuteReader(CommandType.Text,
					"SELECT * FROM Patches"))
			{
				DisplayRowValues(reader);
			}
		}

		static void DisplayRowValues(IDataReader reader)
		{
			while (reader.Read())
			{
				for (int i = 0; i < reader.FieldCount; i++)
				{
					Trace.WriteLine(string.Format("{0} = {1}", reader.GetName(i), reader[i].ToString()));
				}
				Trace.WriteLine("");
			}
		}
	}
}