using System.Linq;
using System.ServiceProcess;
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
	public sealed class Bootstrapper
	{
		private static readonly Lazy<Bootstrapper> _instance = new Lazy<Bootstrapper>(() => new Bootstrapper());

		public static Bootstrapper Instance
		{
			get { return _instance.Value; }
		}

		private Thread _windowThread;
		private MainViewModel _mainViewModel;
		private readonly AutoResetEvent _mainViewStartedEvent = new AutoResetEvent(false);
		private CancellationTokenSource _connectToServiceCancellationTokenSource;

		private Bootstrapper()
		{
			_mainViewStartedEvent = new AutoResetEvent(false);
			_connectToServiceCancellationTokenSource = new CancellationTokenSource();
		}

		public void Run(ILicenseManager licenseManager)
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

				ServiceRepository.Instance.WindowsServiceStatusMonitor.StatusChanged -= WindowsServiceStatusMonitorOnStatusChanged;
				ServiceRepository.Instance.WindowsServiceStatusMonitor.StatusChanged += WindowsServiceStatusMonitorOnStatusChanged;

				if (!ServiceRepository.Instance.WindowsServiceStatusMonitor.Start())
					MessageBoxService.ShowWarning("Служба \"Сервер приложений A.C.Tech\" не обнаружена.\nДля управления службой установите ее и перезапустите монитор сервера");
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове Bootstrapper.Run");
				Close();
			}
		}

		private void WindowsServiceStatusMonitorOnStatusChanged(ServiceControllerStatus status)
		{
			switch (status)
			{
				case ServiceControllerStatus.Running:
					// Изменяем статус состояния Сервера на "Запускается"
					ServiceRepository.Instance.ServiceStateHolder.State = ServiceState.Starting;
					// Пытаемся зарегистрироваться на Сервере в качестве Клиента
					StartConnectToServiceTask();
					break;
				case ServiceControllerStatus.Stopped:
					// Прерываем задачу регистрации на Сервере в качестве Клиента, если она еще не закончилась
					StopConnectToServerTask();
					// Изменяем статус состояния Сервера на "Остановлен"
					ServiceRepository.Instance.ServiceStateHolder.State = ServiceState.Stoped;

					if (FiresecManager.FiresecService != null)
						FiresecManager.FiresecService.StopPoll();
					break;
			}
		}

		private void StartConnectToServiceTask()
		{
			_connectToServiceCancellationTokenSource = new CancellationTokenSource();
			// Регистрируемся на Сервере в качестве Клиента
			Task.Factory.StartNew(() => ConnectToServiceTask(_connectToServiceCancellationTokenSource.Token), _connectToServiceCancellationTokenSource.Token);
		}

		private void StopConnectToServerTask()
		{
			if (_connectToServiceCancellationTokenSource != null)
				_connectToServiceCancellationTokenSource.Cancel();
		}

		private void ConnectToServiceTask(CancellationToken cancellationToken)
		{
			Logger.Info("Попытка регистрации на Сервере в качестве Клиента");
			while (!string.IsNullOrEmpty(FiresecManager.Connect(ClientType.ServiceMonitor, string.Format("net.pipe://{0}/{1}/", NetworkHelper.LocalhostIp, AppServerServices.ServiceName), null, null)))
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				cancellationToken.ThrowIfCancellationRequested();
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
		}

		private void OnWorkThread(object o)
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

		public void Close()
		{
			// Останавливаем слежение за Windows-службой "StrazhService"
			ServiceRepository.Instance.WindowsServiceStatusMonitor.Stop();

			// Разрегистрируемся на Сервере и прекращаем прием сообщений от него
			FiresecManager.Disconnect();
			
			if (_windowThread != null)
			{
				_windowThread.Interrupt();
				_windowThread = null;
			}
			Environment.Exit(1);
		}
	}
}