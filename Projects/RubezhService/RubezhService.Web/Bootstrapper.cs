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
using RubezhService.Models;

namespace RubezhService
{
	public static class Bootstrapper
	{
		public static void Run()
		{
			try
			{
				Notifier.SetNotifier(new RubezhNotifier());
				//Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				ServiceBootstrapper.Run();
				Logger.Trace(SystemInfo.GetString());

				RubezhService.Service.RubezhService.ServerState = ServerState.Starting;
				LogModel.AddLog("Проверка лицензии");
				if (!RubezhLicenseProcessor.TryLoadLicense())
					LogModel.AddLog("Ошибка лицензии", true);
				LicenseModel.Initialize();

				LogModel.AddLog("Проверка соединения с БД");
				using (var dbService = new DbService())
				{
					if (dbService.CheckConnection().HasError)
						LogModel.AddLog("Ошибка соединения с БД", true);
				}

				LogModel.AddLog("Загрузка конфигурации");
				ConfigurationCashHelper.Update();

				if (UACHelper.IsAdministrator)
				{
					LogModel.AddLog("Открытие хоста");
					RubezhServiceManager.Open(false);

					GKProcessor.Create();
					LogModel.AddLog("Запуск ГК");
					GKModel.Initialize();
					GKProcessor.Start();

					AutomationProcessor.Start();
					ScheduleRunner.Start();
					ServerTaskRunner.Start();
					AutomationProcessor.RunOnApplicationRun();
					ClientsManager.StartRemoveInactiveClients(TimeSpan.FromDays(1));
					LogModel.AddLog("Готово");
				}
				else
					LogModel.AddLog("Для запуска сервера требуются права администратора", true);

				RubezhService.Service.RubezhService.ServerState = ServerState.Ready;

			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				LogModel.AddLog("Ошибка при запуске сервера", true);
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