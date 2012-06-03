using System;
using System.IO;
using System.Threading;
using System.Windows;
using Common;
using Infrastructure.Common.MessageBox;
using FiresecService.ViewModels;
using FiresecServiceRunner;
using Infrastructure.Common;
using FiresecService.Service;

namespace FiresecService
{
	public static class Bootstrapper
	{
		static Thread WindowThread;
		static bool IsHostOpened = false;

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

				var synchronisationHostWindow = new Window()
				{
					WindowStyle = System.Windows.WindowStyle.None,
					Width = 0,
					Height = 0,
					ShowInTaskbar = false
				};
				synchronisationHostWindow.Show();
				synchronisationHostWindow.Hide();

				IsHostOpened = FiresecServiceManager.Open();

				WindowThread = new Thread(new ThreadStart(OnWorkThread));
				WindowThread.SetApartmentState(ApartmentState.STA);
				WindowThread.IsBackground = true;
				WindowThread.Start();
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
			var mainWindow = new MainWindow();
			var mainViewModel = new MainViewModel();
			mainViewModel.Satus = IsHostOpened ? "Хост сервиса открыт" : "Хост сервиса НЕ открыт";
			mainWindow.DataContext = mainViewModel;
			try
			{
				mainWindow.Show();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.OnWorkThread");
			}
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			//if (Application.Current != null)
			//    Application.Current.Shutdown();

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