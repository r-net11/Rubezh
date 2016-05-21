using System.Threading.Tasks;
using Common;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using KeyGenerator;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using StrazhAPI.Models;
using StrazhService.Monitor.Events;
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

				// Регистрируемся на Сервере в качестве Клиента
				Task.Factory.StartNew(() =>
				{
					Logger.Info("Попытка регистрации на Сервере в качестве Клиента");
					while (!string.IsNullOrEmpty(FiresecManager.Connect(ClientType.ServiceMonitor, string.Format("net.pipe://{0}/{1}/", NetworkHelper.LocalhostIp, AppServerServices.ServiceName), null, null)))
					{
						Thread.Sleep(TimeSpan.FromSeconds(1));
						Logger.Info("Очередная попытка регистрации на Сервере в качестве Клиента");
					}
					Logger.Info("Зарегистрировались на Сервере в качестве Клиента");

					// Изменяем статус состояния Сервера на "Запущен"
					ServiceRepository.Instance.ServiceStateHolder.State = ServiceState.Started;

					// Получаем лог загрузки Сервера
					var getLogsOperationResult = FiresecManager.FiresecService.GetLogs();
					if (!getLogsOperationResult.HasError)
						ServiceRepository.Instance.Events.GetEvent<ServerLogsReceivedEvent>().Publish(getLogsOperationResult.Result);

					FiresecManager.StartPoll();
					Logger.Info("Начат прием сообщений от Сервера");
				});
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
			Logger.Info("Разрегистрируемся на Сервере и прекращаем прием сообщений от Сервера ");
			FiresecManager.Disconnect();
			
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