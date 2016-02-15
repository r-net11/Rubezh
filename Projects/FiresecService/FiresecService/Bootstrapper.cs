using Common;
using FiresecService.Processor;
using FiresecService.Report;
using FiresecService.Service;
using FiresecService.ViewModels;
using Infrastructure.Automation;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

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
				ServiceFactoryBase.ResourceService.AddResource(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml");
				ServiceFactoryBase.ResourceService.AddResource(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml");
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

				UILogger.Log("Загрузка конфигурации");
				ConfigurationCashHelper.Update();

				UILogger.Log("Открытие хоста");
				FiresecServiceManager.Open();
				ServerLoadHelper.SetStatus(FSServerState.Opened);

				ProcedureExecutionContext.Initialize(
					ContextType.Server,
					() => { return ConfigurationCashHelper.SystemConfiguration; },
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
					ProcedureHelper.ControlPumpStation,
					ProcedureHelper.ControlMPT,
					ProcedureHelper.ExportJournal,
					ProcedureHelper.ExportOrganisation,
					ProcedureHelper.ExportOrganisationList,
					ProcedureHelper.ExportConfiguration,
					ProcedureHelper.ImportOrganisation,
					ProcedureHelper.ImportOrganisationList,
					GetOrganisations
					);

				OpcDaHelper.Initialize(ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaTsServers, ReadTagValue, WriteTagValue);

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

				AutomationProcessor.Start();
				RviProcessor.Start();
				ScheduleRunner.Start();
				ServerTaskRunner.Start();
				AutomationProcessor.RunOnServerRun();
				ClientsManager.StartRemoveInactiveClients(TimeSpan.FromDays(1));
				UILogger.Log("Готово");
				OpcDaServersProcessor.Start();
				UILogger.Log("Запуск OPC DA");
				FiresecService.Service.FiresecService.ServerState = ServerState.Ready;
				FiresecService.Service.FiresecService.AfterConnect += FiresecService_AfterConnect;
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

		static void WriteTagValue(Guid tagUID, object value)
		{
			string error;
			OpcDaServersProcessor.WriteTag(tagUID, value, out error);
		}

		static void FiresecService_AfterConnect(Guid clientUID)
		{
			foreach (var tag in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaTsServers.SelectMany(x => x.Tags))
				FiresecService.Service.FiresecService.NotifyAutomation(new AutomationCallbackResult
				{
					CallbackUID = Guid.NewGuid(),
					ContextType = ContextType.Server,
					AutomationCallbackType = AutomationCallbackType.OpcDaTag,
					Data = new OpcDaTagCallBackData
					{
						TagUID = tag.Uid,
						Value = OpcDaHelper.GetTagValue(tag.Uid)
					}
				}, clientUID);

			foreach (var variable in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables)
				FiresecService.Service.FiresecService.NotifyAutomation(new AutomationCallbackResult
				{
					CallbackUID = Guid.NewGuid(),
					ContextType = ContextType.Server,
					AutomationCallbackType = AutomationCallbackType.GlobalVariable,
					Data = new GlobalVariableCallBackData
					{
						VariableUID = variable.Uid,
						Value = variable.Value
					}
				}, clientUID);
		}

		static List<RubezhAPI.SKD.Organisation> GetOrganisations(Guid clientUID)
		{
			var result = FiresecServiceManager.SafeFiresecService.GetOrganisations(clientUID, new RubezhAPI.SKD.OrganisationFilter());
			return result.HasError ? new List<RubezhAPI.SKD.Organisation>() : result.Result;
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
			System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
		}
	}
}