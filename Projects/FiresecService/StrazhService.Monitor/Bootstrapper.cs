using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using KeyGenerator;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using StrazhService.Monitor.ViewModels;

namespace StrazhService.Monitor
{
	public static class Bootstrapper
	{
		private static Thread _windowThread;
		private static MainViewModel _mainViewModel;
		private static readonly AutoResetEvent _mainViewStartedEvent = new AutoResetEvent(false);

		public static void Run(ILicenseManager licenseManager)
		{
			if (licenseManager == null)
				throw new ArgumentNullException("licenseManager");

			try
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Logger.Trace(SystemInfo.GetString());
				var resourceService = new ResourceService();
				resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
				resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
				_windowThread = new Thread(OnWorkThread)
				{
					Name = "Main window",
					Priority = ThreadPriority.Highest
				};
				_windowThread.SetApartmentState(ApartmentState.STA);
				_windowThread.IsBackground = true;
				_windowThread.Start(licenseManager);
				_mainViewStartedEvent.WaitOne();

				// При смене лицензии Сервера производим валидацию конфигурации Сервера на соответствие новой лицензии
				// и уведомляем всех Клиентов
				licenseManager.LicenseChanged += () =>
				{
					//TODO
				};

			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
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
			}
			_mainViewStartedEvent.Set();
			System.Windows.Threading.Dispatcher.Run();
		}

		public static void Close()
		{
			if (_windowThread != null)
			{
				_windowThread.Interrupt();
				_windowThread = null;
			}
			Environment.Exit(1);

#if DEBUG
			return;
#endif
			Process.GetCurrentProcess().Kill();
		}
	}
}