using Common;
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
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				ServiceBootstrapper.Run();
				Logger.Trace(SystemInfo.GetString());

				RubezhService.Service.RubezhService.ServerState = ServerState.Starting;
				LogPresenter.AddLog("Проверка лицензии");
				if (!RubezhLicenseProcessor.TryLoadLicense())
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

				if (UACHelper.IsAdministrator)
				{
					LogPresenter.AddLog("Открытие хоста");
					RubezhServiceManager.Open(false);

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
				}
				else
					LogPresenter.AddLog("Для запуска сервера требуются права администратора", true);

				RubezhService.Service.RubezhService.ServerState = ServerState.Ready;

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