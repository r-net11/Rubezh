using Infrastructure.Common;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Content;

namespace FireMonitor.Multiclient
{
	public class ServiceFactory : ServiceFactoryBase
	{
		public static void Initialize()
		{
			Events = new EventAggregator();
			ResourceService = new ResourceService();
			ContentService = new ContentService("Monitor");
		}
	}
}