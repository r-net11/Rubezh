using System;
using System.Collections.ObjectModel;
using System.ServiceProcess;
using Common;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhService.Monitor.ViewModels
{
	public class ServiceControlViewModel : BaseViewModel
	{
		private const string ServiceName = "StrazhService";
		private const int ServiceWaitTimeout = 10;
		private ServiceController _serviceController;

		public ServiceControlViewModel()
		{
			_serviceController = new ServiceController(ServiceName);

			StartServiceCommand = new RelayCommand(OnStartService, CanStartService);
			StopServiceCommand = new RelayCommand(OnStopService, CanStopService);
			RestartServiceCommand = new RelayCommand(OnRestartService, CanRestartService);
		}

		public RelayCommand StartServiceCommand { get; private set; }
		private void OnStartService()
		{
			try
			{
				_serviceController.Start();
				_serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(ServiceWaitTimeout));
				if (_serviceController.Status != ServiceControllerStatus.Running)
					Logger.Warn(string.Format("Служба '{0}' не была запущена за {1} сек.", ServiceName, ServiceWaitTimeout));
				OnPropertyChanged(() => StartServiceCommand);
				OnPropertyChanged(() => StopServiceCommand);
			}
			catch (Exception e)
			{
				Logger.Error(e, string.Format("Ошибка запуска службы '{0}'", ServiceName));
			}
		}
		private bool CanStartService()
		{
			return ServiceRepository.Instance.WindowsServiceStatusMonitor.IsStarted
				&& ServiceRepository.Instance.WindowsServiceStatusMonitor.Status == ServiceControllerStatus.Stopped;
		}

		public RelayCommand StopServiceCommand { get; private set; }
		private void OnStopService()
		{
			try
			{
				_serviceController.Stop();
				_serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(ServiceWaitTimeout));
				if (_serviceController.Status != ServiceControllerStatus.Stopped)
					Logger.Warn(string.Format("Служба '{0}' не была остановлена за {1} сек.", ServiceName, ServiceWaitTimeout));
				OnPropertyChanged(() => StartServiceCommand);
				OnPropertyChanged(() => StopServiceCommand);
			}
			catch (Exception e)
			{
				Logger.Error(e, string.Format("Ошибка остановки службы '{0}'", ServiceName));
			}
		}
		private bool CanStopService()
		{
			return ServiceRepository.Instance.WindowsServiceStatusMonitor.IsStarted
				&& ServiceRepository.Instance.WindowsServiceStatusMonitor.Status == ServiceControllerStatus.Running;
		}

		public RelayCommand RestartServiceCommand { get; private set; }
		private void OnRestartService()
		{
			OnStopService();
			OnStartService();
		}
		private bool CanRestartService()
		{
			return ServiceRepository.Instance.WindowsServiceStatusMonitor.IsStarted
				&& ServiceRepository.Instance.WindowsServiceStatusMonitor.Status == ServiceControllerStatus.Running;
		}
	}
}