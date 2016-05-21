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
			RestartServiceCommand = new RelayCommand(OnRestartService);
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
				ServiceRepository.Instance.ServiceStateHolder.State = ServiceState.Starting;
			}
			catch (Exception e)
			{
				Logger.Error(e, string.Format("Ошибка запуска службы '{0}'", ServiceName));
			}
		}
		private bool CanStartService()
		{
			return _serviceController.Status == ServiceControllerStatus.Stopped;
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
				ServiceRepository.Instance.ServiceStateHolder.State = ServiceState.Stoped;
			}
			catch (Exception e)
			{
				Logger.Error(e, string.Format("Ошибка остановки службы '{0}'", ServiceName));
			}
		}
		private bool CanStopService()
		{
			return _serviceController.Status == ServiceControllerStatus.Running;
		}

		public RelayCommand RestartServiceCommand { get; private set; }
		private void OnRestartService()
		{
			try
			{
				if (_serviceController.Status == ServiceControllerStatus.Running)
					_serviceController.Stop();
				_serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(ServiceWaitTimeout));
				if (_serviceController.Status != ServiceControllerStatus.Stopped)
					Logger.Warn(string.Format("В процессе перезапуска служба '{0}' не была остановлена за {1} сек.", ServiceName, ServiceWaitTimeout));
				else
				{
					_serviceController.Start();
					_serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(ServiceWaitTimeout));
					if (_serviceController.Status != ServiceControllerStatus.Running)
						Logger.Warn(string.Format("В процессе перезапуска служба '{0}' не была запущена за {1} сек.", ServiceName, ServiceWaitTimeout));
				}
				OnPropertyChanged(() => StartServiceCommand);
				OnPropertyChanged(() => StopServiceCommand);
			}
			catch (Exception e)
			{
				Logger.Error(e, string.Format("Ошибка перезапуска службы '{0}'", ServiceName));
			}
		}
	}
}