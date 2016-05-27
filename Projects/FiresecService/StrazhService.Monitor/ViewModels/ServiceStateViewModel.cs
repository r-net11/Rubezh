using System;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhService.Monitor.ViewModels
{
	public class ServiceStateViewModel : BaseViewModel
	{
		private ServiceState _serviceState;

		public ServiceState ServiceState
		{
			get { return _serviceState; }
			set
			{
				if (_serviceState == value)
					return;
				_serviceState = value;
				OnPropertyChanged(() => ServiceState);
			}
		}

		public ServiceStateViewModel()
		{
			ServiceRepository.Instance.ServiceStateHolder.ServiceStateChanged -= OnServiceStateChanged;
			ServiceRepository.Instance.ServiceStateHolder.ServiceStateChanged += OnServiceStateChanged;
		}

		private void OnServiceStateChanged(ServiceState serviceState)
		{
			ServiceState = serviceState;
		}
	}
}