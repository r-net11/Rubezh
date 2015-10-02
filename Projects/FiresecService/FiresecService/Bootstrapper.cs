using System;
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
using FiresecAPI;
using SKDDriver.DataClasses;
using FiresecService.Processor;
using Infrastructure.Automation;
using FiresecAPI.AutomationCallback;
using FiresecAPI.Automation;

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

				FiresecService.Service.FiresecService.ServerState = ServerState.Starting;

                UILogger.Log("Проверка лицензии");
                if (!FiresecLicenseProcessor.TryLoadLicense())
                    UILogger.Log("Ошибка лицензии", true);

				UILogger.Log("Проверка соединения с БД");
				using (var dbService = new DbService())
				{
					if (dbService.CheckConnection().HasError)
					UILogger.Log("Ошибка соединения с БД", true);
				}
				
				UILogger.Log("Открытие хоста");
				FiresecServiceManager.Open();
				ServerLoadHelper.SetStatus(FSServerState.Opened);

				UILogger.Log("Загрузка конфигурации");
				ConfigurationCashHelper.Update();

				ProcedureExecutionContext.Initialize(
					ContextType.Server,
					ConfigurationCashHelper.SystemConfiguration,
					ConfigurationCashHelper.SecurityConfiguration,
					Service.FiresecService.NotifyAutomation,
					null,
					null,
					ProcedureHelper.AddJournalItem,
					ProcedureHelper.ControlGKDevice,
					ProcedureHelper.StartRecord,
					ProcedureHelper.StopRecord,
					ProcedureHelper.Ptz,
					ProcedureHelper.RviAlarm,
					ProcedureHelper.ControlFireZone,
					ProcedureHelper.ControlGuardZone,
					ProcedureHelper.ControlDirection,
					ProcedureHelper.ControlGKDoor,
					ProcedureHelper.ControlDelay,
					ProcedureHelper.ExportJournal,
					ProcedureHelper.ExportOrganisation,
					ProcedureHelper.ExportOrganisationList,
					ProcedureHelper.ExportConfiguration,
					ProcedureHelper.ImportOrganisation,
					ProcedureHelper.ImportOrganisationList
					);

				GKProcessor.Create();
				UILogger.Log("Запуск ГК");
				GKProcessor.Start();

				UILogger.Log("Запуск сервиса отчетов");
				if (ReportServiceManager.Run())
				{
					UILogger.Log("Сервис отчетов запущен: " + ConnectionSettingsManager.ReportServerAddress);
					MainViewModel.SetReportAddress(ConnectionSettingsManager.ReportServerAddress);
				}
				else
				{
					UILogger.Log("Ошибка при запуске сервиса отчетов", true);
					MainViewModel.SetReportAddress("<Ошибка>");
				}
								
				ScheduleRunner.Start();
				ServerTaskRunner.Start();
				AutomationProcessor.RunOnServerRun();
                ClientsManager.StartRemoveInactiveClients(TimeSpan.FromMinutes(10));
				UILogger.Log("Готово");
				FiresecService.Service.FiresecService.ServerState = ServerState.Ready;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера", true);
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
#else
			Process.GetCurrentProcess().Kill();
#endif
		}
	}
}