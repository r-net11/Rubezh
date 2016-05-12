//#define WINDOWS
using Common;
using FiresecService.Presenters;
using FiresecService.Processor;
using FiresecService.Report;
using FiresecService.Service;
//using FiresecService.ViewModels;
using Infrastructure.Automation;
using Infrastructure.Common;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhDAL.DataClasses;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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

				UILogger.Log("Загрузка конфигурации");
				ConfigurationCashHelper.Update();

				UILogger.Log("Открытие хоста");
#if WINDOWS
				FiresecServiceManager.Open(true);
#else
				FiresecServiceManager.Open(false);
#endif
				ServerLoadHelper.SetStatus(FSServerState.Opened);
#if WINDOWS
				OpcDaHelper.Initialize(ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaTsServers, ReadTagValue, WriteTagValue);
#endif
				GKProcessor.Create();
				UILogger.Log("Запуск ГК");
				GKProcessor.Start();
#if WINDOWS
				UILogger.Log("Запуск сервиса отчетов");
				if (ReportServiceManager.Run())
				{
					UILogger.Log("Сервис отчетов запущен: " + ConnectionSettingsManager.ReportServerAddress);
					MainPresenter.SetReportAddress(ConnectionSettingsManager.ReportServerAddress);
				}
				else
				{
					UILogger.Log("Ошибка при запуске сервиса отчетов", true);
					MainPresenter.SetReportAddress("<Ошибка>");
				}
#endif
				AutomationProcessor.Start();
				ScheduleRunner.Start();
				ServerTaskRunner.Start();
				AutomationProcessor.RunOnApplicationRun();
				ClientsManager.StartRemoveInactiveClients(TimeSpan.FromDays(1));
#if WINDOWS
				UILogger.Log("Запуск OPC DA");
				OpcDaServersProcessor.Start();
#endif
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

		[Conditional("WINDOWS")]
		static void ReadTagValue(Guid tagUID, object value)
		{
			OpcDaHelper.SetTagValue(tagUID, value);
			FiresecService.Service.FiresecService.NotifyAutomation(new AutomationCallbackResult
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

		[Conditional("WINDOWS")]
		static void WriteTagValue(Guid tagUID, object value)
		{
			string error;
			OpcDaServersProcessor.WriteTag(tagUID, value, out error);
		}

		public static void Close()
		{
			ServerLoadHelper.SetStatus(FSServerState.Closed);
			System.Environment.Exit(1);
#if DEBUG
			return;
#else
			System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
		}
	}
}