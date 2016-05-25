using Common;
using FiresecService.Report;
using FiresecService.Service;
using FiresecService.Service.Validators;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Windows;
using Integration.Service;
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

				var integrationService = new IntegrationFacade();

				//UILogger.Log("Загрузка конфигурации");
                //UILogger.Log((string)Application.Current.FindResource("lang_LoadConfiguration"));
                UILogger.Log(Resources.Language.Bootstrapper.LoadConfiguration);
				ConfigurationCashHelper.Update();

				//UILogger.Log("Открытие хоста");
                //UILogger.Log((string)Application.Current.FindResource("lang_HostOpening"));
                UILogger.Log(Resources.Language.Bootstrapper.HostOpening);

				try
				{
					FiresecServiceManager.Open(licenseManager, integrationService);
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
				_mainViewModel = new MainViewModel(license);
				ApplicationService.Run(_mainViewModel, false, false);
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