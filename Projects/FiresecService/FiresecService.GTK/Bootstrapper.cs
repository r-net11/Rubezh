using System;
using System.Reflection;
using Common;
using FiresecService.Processor;
using FiresecService.Report;
using FiresecService.Service;
using Infrastructure.Automation;
using Infrastructure.Common;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhDAL.DataClasses;
using System.IO;
using FiresecService.View;
using FiresecService.Views;

namespace FiresecService
{
	public static class Bootstrapper
	{
		public static void Run()
		{
			try
			{
				Notifier.SetNotifier(new FiresecNotifier());
				ServiceBootstrapper.Run();
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Logger.Trace(SystemInfo.GetString());

				Service.FiresecService.ServerState = ServerState.Starting;

				UILogger.Log("Проверка лицензии");
				if (!FiresecLicenseProcessor.TryLoadLicense())
					UILogger.Log("Ошибка лицензии", true);

				UILogger.Log("Проверка соединения с БД");
				using (var dbService = new DbService())
				{
					if (dbService.CheckConnection().HasError)
						UILogger.Log("Ошибка соединения с БД", true);
				}

				UILogger.Log("Загрузка конфигурации");
				ConfigurationCashHelper.Update();

				UILogger.Log("Открытие хоста");
				FiresecServiceManager.Open();
				ServerLoadHelper.SetStatus(FSServerState.Opened);

				OpcDaHelper.Initialize(ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaTsServers, ReadTagValue, WriteTagValue);

				GKProcessor.Create();
				UILogger.Log("Запуск ГК");
				GKProcessor.Start();

				UILogger.Log("Запуск сервиса отчетов");
				if (ReportServiceManager.Run())
				{
					UILogger.Log("Сервис отчетов запущен: " + ConnectionSettingsManager.ReportServerAddress);
					MainView.Current.SetReportAddress(ConnectionSettingsManager.ReportServerAddress);
				}
				else
				{
					UILogger.Log("Ошибка при запуске сервиса отчетов", true);
					MainView.Current.SetReportAddress("<Ошибка>");
				}

				AutomationProcessor.Start();
				ScheduleRunner.Start();
				ServerTaskRunner.Start();
				AutomationProcessor.RunOnApplicationRun();
				ClientsManager.StartRemoveInactiveClients(TimeSpan.FromDays(1));
				UILogger.Log("Запуск OPC DA");
				OpcDaServersProcessor.Start();
				UILogger.Log("Готово");

				Service.FiresecService.ServerState = ServerState.Ready;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера", true);
				Close();
			}
		}

		static void ReadTagValue(Guid tagUID, object value)
		{
			OpcDaHelper.SetTagValue(tagUID, value);
			Service.FiresecService.NotifyAutomation(new AutomationCallbackResult
			{
				CallbackUID = Guid.NewGuid(),
				ContextType = ContextType.Server,
				AutomationCallbackType = AutomationCallbackType.OpcDaTag,
				Data = new OpcDaTagCallBackData
				{
					TagUID = tagUID,
					Value = value
				}
			}, null);
		}

		static void WriteTagValue(Guid tagUID, object value)
		{
			string error;
			OpcDaServersProcessor.WriteTag(tagUID, value, out error);
		}

		public static void Close()
		{
			ServerLoadHelper.SetStatus(FSServerState.Closed);
			Environment.Exit(1);
#if !DEBUG
			System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
		}
	}
}
