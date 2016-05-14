using Common;
using FiresecService.Presenters;
using FiresecService.Processor;
using FiresecService.Service;
using Infrastructure.Automation;
using Infrastructure.Common;
using RubezhAPI;
using RubezhDAL.DataClasses;
using System;
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

				if (UACHelper.IsAdministrator)
				{
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

					FiresecServiceManager.Open(false);

					GKProcessor.Create();
					UILogger.Log("Запуск ГК");
					GKProcessor.Start();
					AutomationProcessor.Start();
					ScheduleRunner.Start();
					ServerTaskRunner.Start();
					AutomationProcessor.RunOnApplicationRun();
					ClientsManager.StartRemoveInactiveClients(TimeSpan.FromDays(1));

					UILogger.Log("Готово");

					FiresecService.Service.FiresecService.ServerState = ServerState.Ready;
				}
				else
					UILogger.Log("Для запуска сервера требуются права администратора", true);

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