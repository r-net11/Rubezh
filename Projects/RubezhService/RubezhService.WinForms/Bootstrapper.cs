using Common;
using RubezhService.Presenters;
using RubezhService.Processor;
using RubezhService.Service;
using Infrastructure.Automation;
using Infrastructure.Common;
using RubezhAPI;
using RubezhDAL.DataClasses;
using System;
using System.IO;
using System.Reflection;

namespace RubezhService
{
	public static class Bootstrapper
	{
		public static void Run()
		{
			try
			{
				Notifier.SetNotifier(new RubezhNotifier());
				ServiceBootstrapper.Run();
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Logger.Trace(SystemInfo.GetString());

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

				if (UACHelper.IsAdministrator)
				{
					UILogger.Log("Открытие хоста");

					RubezhServiceManager.Open(false);

					GKProcessor.Create();
					UILogger.Log("Запуск ГК");
					GKProcessor.Start();
					AutomationProcessor.Start();
					ScheduleRunner.Start();
					ServerTaskRunner.Start();
					AutomationProcessor.RunOnApplicationRun();
					ClientsManager.StartRemoveInactiveClients(TimeSpan.FromDays(1));

					UILogger.Log("Готово");

				}
				else
					UILogger.Log("Для запуска сервера требуются права администратора", true);

				RubezhService.Service.RubezhService.ServerState = ServerState.Ready;

			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера", true);
				Close();
			}
		}

		public static void Close()
		{
			System.Environment.Exit(1);
#if DEBUG
			return;
#else
			System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
		}
	}
}