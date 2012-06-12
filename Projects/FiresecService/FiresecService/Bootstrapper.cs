using System;
using System.IO;
using System.Threading;
using System.Windows;
using Common;
using FiresecService.Service;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace FiresecService
{
	public static class Bootstrapper
	{
		static Thread WindowThread = null;
		static MainViewModel MainViewModel;
		static AutoResetEvent MainViewStartedEvent = new AutoResetEvent(false);

		public static void Run()
		{
			SingleLaunchHelper.KillRunningProcess(true);

			try
			{
				InitializeAppSettings();
				var directoryInfo = new DirectoryInfo(Environment.GetCommandLineArgs()[0]);
				Environment.CurrentDirectory = directoryInfo.FullName.Replace(directoryInfo.Name, "");

				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));

				WindowThread = new Thread(new ThreadStart(OnWorkThread));
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();
				MainViewStartedEvent.WaitOne();

				var comServersStatus = ClientsCash.InitializeComServers();
				var isHostOpened = FiresecServiceManager.Open();
				MainViewModel.Current.UpdateStatus
					(isHostOpened ? "Хост сервиса открыт" : "Хост сервиса НЕ открыт",
					comServersStatus ? "Соединение с ядром Firesec установлено" : "Соединение с ядром Firesec НЕ установлено"
					);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				MessageBoxService.ShowException(e);
				Close();
			}
		}

		static void OnWorkThread()
		{
			try
			{
				MainViewModel = new MainViewModel();
				ApplicationService.Run(MainViewModel);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");
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
		}

		static void InitializeAppSettings()
		{
			AppSettings.OldFiresecLogin = System.Configuration.ConfigurationManager.AppSettings["OldFiresecLogin"] as string;
			AppSettings.OldFiresecPassword = System.Configuration.ConfigurationManager.AppSettings["OldFiresecPassword"] as string;
			AppSettings.ServiceAddress = System.Configuration.ConfigurationManager.AppSettings["ServiceAddress"] as string;
			AppSettings.OverrideFiresec1Config = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["OverrideFiresec1Config"] as string);
#if DEBUG
			AppSettings.IsDebug = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsDebug"] as string);
#endif
		}
	}
}