using System;
using Common;
using FiresecService;
using FiresecService.Report;
using FiresecService.Service;
using FiresecService.Service.Validators;
using KeyGenerator;

namespace StrazhService.WS
{
	public class ServiceStarter
	{
		public void Start()
		{
			Logger.Info("Запуск сервера");

			// Актуализация схемы БД
			try
			{
				Logger.Info("Актуализация схемы БД");
				PatchManager.Patch();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Ошибка актуализации схемы БД");
			}

			try
			{
				// Проверка наличия лицензии
				Logger.Info("Проверка наличия лицензии");
				var licenseManager = new LicenseManager();
				if (!licenseManager.IsValidExistingKey())
				{
					Logger.Error("Лицензия отсутствует");
				}

				// Инициализируем валидатор конфигурации
				Logger.Info("Инициализируем валидатор конфигурации");
				ConfigurationElementsAgainstLicenseDataValidator.Instance.LicenseManager = licenseManager;

				Logger.Info("Загрузка конфигурации");
				ConfigurationCashHelper.Update();

				Logger.Info("Открытие хоста");
				if (!FiresecServiceManager.Open(licenseManager))
					Logger.Error("При открытии хоста обнаружена ошибка");

				Logger.Info("Создание конфигурации СКД");
				SKDProcessor.Start();

				Logger.Info("Запуск сервиса отчетов");
				ReportServiceManager.Run();
				Logger.Info("Сервис отчетов запущен" + ReportServiceManager.Address);
				ReportServiceManager.Addresses.ForEach(Logger.Info);

				Logger.Info("Запуск автоматизации");
				ScheduleRunner.Start();

				Logger.Info("Готово");
				ProcedureRunner.RunOnServerRun();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Ошибка при запуске сервера приложений");
				Stop();
			}
		}

		public void Stop()
		{
			Logger.Info("Остановка сервера");
			ProcedureRunner.Terminate();
			Environment.Exit(1);
		}
	}
}