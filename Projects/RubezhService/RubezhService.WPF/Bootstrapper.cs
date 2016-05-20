using Common;
using RubezhService.Models;
using RubezhService.Processor;
using RubezhService.Report;
using RubezhService.Service;
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
using System.IO;
using System.Reflection;
using System.Threading;

namespace RubezhService
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
				Notifier.SetNotifier(new RubezhNotifier());
				ServiceBootstrapper.Run();

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

				RubezhService.Service.RubezhService.ServerState = ServerState.Starting;

				UILogger.Log("Проверка лицензии");
				if (!RubezhLicenseProcessor.TryLoadLicense())
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
				RubezhServiceManager.Open();
				ServerLoadHelper.SetStatus(FSServerState.Opened);

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
				AutomationProcessor.RunOnApplicationRun();
				ClientsManager.StartRemoveInactiveClients(TimeSpan.FromDays(1));
				UILogger.Log("Запуск OPC DA");
				OpcDaServersProcessor.Start();
				UILogger.Log("Готово");
				RubezhService.Service.RubezhService.ServerState = ServerState.Ready;
				ApplicationService.Closing += ApplicationService_Closing;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера", true);
				Close();
			}
		}

		static void ApplicationService_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!e.Cancel)
				Notifier.OnApplicationClosing();
		}

		static void ReadTagValue(Guid tagUID, object value)
		{
			OpcDaHelper.SetTagValue(tagUID, value);
			RubezhService.Service.RubezhService.NotifyAutomation(new AutomationCallbackResult
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

		static void OnWorkThread()
		{
			try
			{
				MainViewModel = new MainViewModel();
				ApplicationService.Run(MainViewModel, false);
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