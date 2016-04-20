using Infrastructure.Common.Windows;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.Services.Content;

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