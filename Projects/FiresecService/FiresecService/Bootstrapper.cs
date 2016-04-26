using Common;
using Controls.Converters;
using FiresecAPI.Models;
using FiresecService.Report;
using FiresecService.Service;
using FiresecService.Service.Validators;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Services.Configuration;
using Infrastructure.Common.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using KeyGenerator;
using KeyGenerator.Entities;

namespace FiresecService
{
	public static class Bootstrapper
	{
		private static Thread WindowThread = null;
		private static MainViewModel MainViewModel;
		private static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

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
				WindowThread = new Thread(OnWorkThread) {Name = "Main window", Priority = ThreadPriority.Highest};
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start(licenseManager);
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
				
				//UILogger.Log("Загрузка конфигурации");
                //UILogger.Log((string)Application.Current.FindResource("lang_LoadConfiguration"));
                UILogger.Log(Resources.Language.Bootstrapper.AutomationStart);
				ConfigurationCashHelper.Update();

				//UILogger.Log("Открытие хоста");
                //UILogger.Log((string)Application.Current.FindResource("lang_HostOpening"));
                UILogger.Log(Resources.Language.Bootstrapper.HostOpening);

				try
				{
					FiresecServiceManager.Open(licenseManager);
				}
				catch (Exception)
				{
					//MessageBox.Show("При открытии хоста обнаружена ошибка");
                    //MessageBox.Show((string)Application.Current.FindResource("lang_HostOpeningError"));
				    MessageBox.Show(Resources.Language.Bootstrapper.HostOpeningError);
					Application.Current.MainWindow.Close();
				}

				//UILogger.Log("Создание конфигурации СКД");
                //UILogger.Log((string)Application.Current.FindResource("lang_ConfigurationCreation"));
                UILogger.Log(Resources.Language.Bootstrapper.ConfigurationCreation);
                try
				{
					SKDProcessor.Start();
				}
				catch (Exception)
				{
                    //MessageBox.Show("В конфигурационном файле SKD содержиться ошибка или он отсутствует");
                    //MessageBox.Show((string)Application.Current.FindResource("lang_SKDProcessorStart_Exception"));
				    MessageBox.Show(Resources.Language.Bootstrapper.SKDProcessorStart_Exception);
					Application.Current.MainWindow.Close();
				}

				//UILogger.Log("Запуск сервиса отчетов");
                //UILogger.Log((string)Application.Current.FindResource("lang_ReportServiceStart"));
                UILogger.Log(Resources.Language.Bootstrapper.ReportServiceStart);

				ReportServiceManager.Run();
				//UILogger.Log("Сервис отчетов запущен" + ReportServiceManager.Address);
                //UILogger.Log((string)Application.Current.FindResource("lang_ReportServiceIsStarted") + ReportServiceManager.Address);
                UILogger.Log(Resources.Language.Bootstrapper.ReportServiceIsStarted);
                ReportServiceManager.Addresses.ForEach(UILogger.Log);

                //UILogger.Log("Запуск автоматизации");
                //UILogger.Log((string)Application.Current.FindResource("lang_AutomationStart"));
                UILogger.Log(Resources.Language.Bootstrapper.AutomationStart);
				ScheduleRunner.Start();

				//UILogger.Log("Готово");
                //UILogger.Log((string)Application.Current.FindResource("lang_Ready"));
                UILogger.Log(Resources.Language.Bootstrapper.Ready);
				ProcedureRunner.RunOnServerRun();
			}
			catch (Exception e)
            {
                //Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
                //UILogger.Log("Ошибка при запуске сервера");
                //BalloonHelper.ShowFromServer("Ошибка во время загрузки");
                //Logger.Error(e, (string)Application.Current.FindResource("lang_CallBootstraperRun_Exception"));
                //UILogger.Log((string)Application.Current.FindResource("lang_ServerStartLog_Error"));
                //BalloonHelper.ShowFromServer((string)Application.Current.FindResource("lang_ServerStartBallon_Error"));
                Logger.Error(e, Resources.Language.Bootstrapper.CallBootstraperRun_Exception);
                UILogger.Log(Resources.Language.Bootstrapper.ServerStartLog_Error);
                BalloonHelper.ShowFromServer(Resources.Language.Bootstrapper.ServerStartBallon_Error);
				Close();
			}
		}

		private static void OnWorkThread(object o)
		{
			var license = o as ILicenseManager;
			if (license == null) return;

			try
			{
				MainViewModel = new MainViewModel(license);
				ApplicationService.Run(MainViewModel, false, false);
			}
			catch (Exception e)
			{
				//Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");
                Logger.Error(e, Resources.Language.Bootstrapper.CallBootstraperOnWork_Exception);
                //BalloonHelper.ShowFromServer("Ошибка во время загрузки");
                BalloonHelper.ShowFromServer(Resources.Language.Bootstrapper.ServerStartBallon_Error);
			}
			MainViewStartedEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			if (WindowThread != null)
			{
				WindowThread.Interrupt();
				WindowThread = null;
			}
			System.Environment.Exit(1);

#if DEBUG
			return;
#endif
			Process.GetCurrentProcess().Kill();
		}
	}
}