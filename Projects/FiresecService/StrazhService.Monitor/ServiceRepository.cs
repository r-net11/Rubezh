using System;
using System.ServiceProcess;
using Microsoft.Practices.Prism.Events;

namespace StrazhService.Monitor
{
	public sealed class ServiceRepository
	{
		private static readonly Lazy<ServiceRepository> _instance = new Lazy<ServiceRepository>(() => new ServiceRepository());
		
		private ServiceRepository()
		{
			Events = new EventAggregator();
			ServiceStateHolder = new ServiceStateHolder();
			WindowsServiceStatusMonitor = new WindowsServiceStatusMonitor(new ServiceController("StrazhService"));
		}

		public static ServiceRepository Instance
		{
			get { return _instance.Value; }
		}

		public IEventAggregator Events { get; private set; }

		public IServiceStateHolder ServiceStateHolder { get; private set; }

		public IWindowsServiceStatusMonitor WindowsServiceStatusMonitor { get; private set; }
	}
}