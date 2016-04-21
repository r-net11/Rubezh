using Common;
using FiresecService.Report;
using FiresecService.Service;
using FiresecService.Service.Validators;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;
using KeyGenerator;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace FiresecService
{
	public static class Bootstrapper
	{
		private static Thread _windowThread;
		private static MainViewModel _mainViewModel;
		private static readonly AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

		public static void Run(ILicenseManager licenseManager)
		{
			if(licenseManager == null)
				throw new ArgumentException("LicenseManager");

			try
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Logger.Trace(SystemInfo.GetString());
				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
				_windowThread = new Thread(OnWorkThread) {Name = "Main window", Priority = ThreadPriority.Highest};
				_windowThread.SetApartmentState(ApartmentState.STA);
				_windowThread.IsBackground = true;
				_windowThread.Start(licenseManager);
				MainViewStartedEvent.WaitOne();

				// Инициализируем валидатор конфигурации
				ConfigurationElementsAgainstLicenseDataValidator.Instance.LicenseManager = licenseManager;

				// При смене лицензии Сервера производим валидацию конфигурации Сервера на соответствие новой лицензии
				// и уведомляем всех Клиентов
				licenseManager.LicenseChanged += () =>
				{
					ConfigurationElementsAgainstLicenseDataValidator.Instance.Validate();
					FiresecServiceManager.SafeFiresecService.NotifyLicenseChanged();
				};

				UILogger.Log("Загрузка конфигурации");

				ConfigurationCashHelper.Update();

				UILogger.Log("Открытие хоста");
				try
				{
					FiresecServiceManager.Open(licenseManager);
				}
				catch (Exception)
				{
					MessageBox.Show("При открытии хоста обнаружена ошибка");
					Application.Current.MainWindow.Close();
				}

				UILogger.Log("Создание конфигурации СКД");
				try
				{
					SKDProcessor.Start();
				}
				catch (Exception)
				{
					MessageBox.Show("В конфигурационном файле SKD содержиться ошибка или он отсутствует");
					Application.Current.MainWindow.Close();
				}

				UILogger.Log("Запуск сервиса отчетов");
				ReportServiceManager.Run();
				UILogger.Log("Сервис отчетов запущен" + ReportServiceManager.Address);
				ReportServiceManager.Addresses.ForEach(UILogger.Log);

				UILogger.Log("Запуск автоматизации");
				ScheduleRunner.Start();

				UILogger.Log("Готово");
				ProcedureRunner.RunOnServerRun();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				UILogger.Log("Ошибка при запуске сервера");
				BalloonHelper.ShowFromServer("Ошибка во время загрузки");
				Close();
			}
		}

		private static void OnWorkThread(object o)
		{
			var license = o as ILicenseManager;
			if (license == null) return;

			try
			{
				_mainViewModel = new MainViewModel(license);
				ApplicationService.Run(_mainViewModel, false, false);
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
			if (_windowThread != null)
			{
				_windowThread.Interrupt();
				_windowThread = null;
			}
			System.Environment.Exit(1);

#if DEBUG
			return;
#endif
			Process.GetCurrentProcess().Kill();
		}
	}
}