using System;
using System.ServiceProcess;
using Microsoft.Practices.Prism.Events;
using StrazhService.Monitor.Database;

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
			DatabaseService = new MsSqlServerDatabaseService();
		}

		public static ServiceRepository Instance
		{
			get { return _instance.Value; }
		}

		public IEventAggregator Events { get; private set; }

		public IServiceStateHolder ServiceStateHolder { get; private set; }

		public IWindowsServiceStatusMonitor WindowsServiceStatusMonitor { get; private set; }

		public IDatabaseService DatabaseService { get; private set; }
	}
}