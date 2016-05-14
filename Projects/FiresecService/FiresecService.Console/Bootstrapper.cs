using Common;
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
				FiresecService.Service.FiresecService.ServerState = ServerState.Starting;
				ServiceBootstrapper.Run();
				Logger.Trace(SystemInfo.GetString());

				if (UACHelper.IsAdministrator)
				{
					Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

					LogPresenter.AddLog("Проверка лицензии");
					if (!FiresecLicenseProcessor.TryLoadLicense())
						LogPresenter.AddLog("Ошибка лицензии", true);
					LicensePresenter.Initialize();

					LogPresenter.AddLog("Проверка соединения с БД");
					using (var dbService = new DbService())
					{
						if (dbService.CheckConnection().HasError)
							LogPresenter.AddLog("Ошибка соединения с БД", true);
					}

					LogPresenter.AddLog("Загрузка конфигурации");
					ConfigurationCashHelper.Update();

					LogPresenter.AddLog("Открытие хоста");
					FiresecServiceManager.Open(false);

					GKProcessor.Create();
					LogPresenter.AddLog("Запуск ГК");
					GKPresenter.Initialize();
					GKProcessor.Start();

					AutomationProcessor.Start();
					ScheduleRunner.Start();
					ServerTaskRunner.Start();
					AutomationProcessor.RunOnApplicationRun();
					ClientsManager.StartRemoveInactiveClients(TimeSpan.FromDays(1));
					LogPresenter.AddLog("Готово");

					FiresecService.Service.FiresecService.ServerState = ServerState.Ready;
				}
				else
					LogPresenter.AddLog("Для запуска сервера требуются права администратора", true);

			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				LogPresenter.AddLog("Ошибка при запуске сервера", true);
				Close();
			}
		}

		public static void Close()
		{
			//ServerLoadHelper.SetStatus(FSServerState.Closed);
			System.Environment.Exit(1);
#if DEBUG
			return;
#else
			System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
		}
	}
}