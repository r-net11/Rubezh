using System;
using Common;
using FiresecService;
using FiresecService.Report;
using FiresecService.Service;
using FiresecService.Service.Validators;
using Integration.Service;
using KeyGenerator;

namespace StrazhService.WS
{
	public class ServiceStarter
	{
		public void Start()
		{
			Notifier.SetNotifier(new StrazhNotifier());

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
				Notifier.Log("Загрузка конфигурации");
				ConfigurationCashHelper.Update();

				Logger.Info("Открытие хоста");
				Notifier.Log("Открытие хоста");
				if (!FiresecServiceManager.Open(licenseManager, new IntegrationFacade()))
					Logger.Error("При открытии хоста обнаружена ошибка");

				Logger.Info("Создание конфигурации СКД");
				Notifier.Log("Создание конфигурации СКД");
				SKDProcessor.Start();

				Logger.Info("Запуск сервиса отчетов");
				Notifier.Log("Запуск сервиса отчетов");
				ReportServiceManager.Run();
				Logger.Info("Сервис отчетов запущен" + ReportServiceManager.Address);
				Notifier.Log("Сервис отчетов запущен" + ReportServiceManager.Address);
				foreach (var reportServiceAddress in ReportServiceManager.Addresses)
				{
					Logger.Info(reportServiceAddress);
					Notifier.Log(reportServiceAddress);
				}

				Logger.Info("Запуск автоматизации");
				Notifier.Log("Запуск автоматизации");
				ScheduleRunner.Start();

				Logger.Info("Готово");
				Notifier.Log("Готово");
				ProcedureRunner.RunOnServerRun();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Ошибка при запуске сервера");
				Stop();
			}
		}

		public void Stop()
		{
			Logger.Info("Остановка сервера");
			ScheduleRunner.Stop();
			ProcedureRunner.Terminate();
			SKDProcessor.Stop();
			FiresecServiceManager.Close();
			ReportServiceManager.Stop();
			ClientsManager.ClientInfos.Clear();
		}
	}
}