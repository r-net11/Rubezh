using System;
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
		}

		public static ServiceRepository Instance
		{
			get { return _instance.Value; }
		}

		public IEventAggregator Events { get; private set; }

		public IServiceStateHolder ServiceStateHolder { get; private set; }
	}
}