using Infrastructure.Common;
using Microsoft.Practices.Prism.Events;

namespace FireMonitor.Multiclient
{
	public class ServiceFactory : ServiceFactoryBase
	{
		public static void Initialize()
		{
			Events = new EventAggregator();
			ResourceService = new ResourceService();
		}
	}
}