using System;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhService.Monitor.ViewModels
{
	public class ServiceControlViewModel : BaseViewModel
	{
		private const string ServiceName = "StrazhService";
		private const int ServiceWaitTimeout = 15;
		private readonly ServiceController _serviceController;
		private bool _isRestarting;
		private readonly Dispatcher _dispatcher;

		public ServiceControlViewModel()
		{
			_dispatcher = Dispatcher.CurrentDispatcher;
			_serviceController = new ServiceController(ServiceName);
			StartServiceCommand = new RelayCommand(OnStartService, CanStartService);
			StopServiceCommand = new RelayCommand(OnStopService, CanStopService);
			RestartServiceCommand = new RelayCommand(OnRestartService, CanRestartService);
		}

		public RelayCommand StartServiceCommand { get; private set; }
		private void OnStartService()
		{
			Task.Factory.StartNew(() =>
			{
				StartService();
				_dispatcher.BeginInvoke((Action)UpdateCommands);
			});
		}
		private bool CanStartService()
		{
			return ServiceRepository.Instance.WindowsServiceStatusMonitor.IsStarted
				&& ServiceRepository.Instance.WindowsServiceStatusMonitor.Status == ServiceControllerStatus.Stopped
				&& !_isRestarting;
		}

		public RelayCommand StopServiceCommand { get; private set; }
		private void OnStopService()
		{
			Task.Factory.StartNew(() =>
			{
				StopService();
				_dispatcher.BeginInvoke((Action)UpdateCommands);
			});
		}
		private bool CanStopService()
		{
			return ServiceRepository.Instance.WindowsServiceStatusMonitor.IsStarted
				&& ServiceRepository.Instance.WindowsServiceStatusMonitor.Status == ServiceControllerStatus.Running
				&& !_isRestarting;
		}

		public RelayCommand RestartServiceCommand { get; private set; }
		private void OnRestartService()
		{
			Task.Factory.StartNew(() =>
			{
				_isRestarting = true;
				StopService();
				StartService();
				_isRestarting = false;
				_dispatcher.BeginInvoke((Action)UpdateCommands);
			});
		}
		private bool CanRestartService()
		{
			return ServiceRepository.Instance.WindowsServiceStatusMonitor.IsStarted
				&& ServiceRepository.Instance.WindowsServiceStatusMonitor.Status == ServiceControllerStatus.Running;
		}

		private void StopService()
		{
			Logger.Info(string.Format("Останавливаем службу '{0}'", ServiceName));
			try
			{
				_serviceController.Stop();
				_serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(ServiceWaitTimeout));
			}
			catch (System.ServiceProcess.TimeoutException)
			{
				Logger.Warn(string.Format("Служба '{0}' не была остановлена за {1} сек.", ServiceName, ServiceWaitTimeout));
			}
			catch (Exception e)
			{
				Logger.Error(e, string.Format("Ошибка остановки службы '{0}'", ServiceName));
			}
		}

		private void StartService()
		{
			Logger.Info(string.Format("Запускаем службу '{0}'", ServiceName));
			try
			{
				_serviceController.Start();
				_serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(ServiceWaitTimeout));
			}
			catch (System.ServiceProcess.TimeoutException)
			{
				Logger.Warn(string.Format("Служба '{0}' не была запущена за {1} сек.", ServiceName, ServiceWaitTimeout));
			}
			catch (Exception e)
			{
				Logger.Error(e, string.Format("Ошибка запуска службы '{0}'", ServiceName));
			}
		}

		private void UpdateCommands()
		{
			CommandManager.InvalidateRequerySuggested();
		}
	}
}