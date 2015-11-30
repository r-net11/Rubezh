using Infrastructure.Common.Services.Content;
using Infrastructure.Common.Services.DragDrop;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Common.Services
{
	public abstract class ServiceFactoryBase
	{
		static ServiceFactoryBase()
		{
			ResourceService = new ResourceService();
		}

		public static IEventAggregator Events { get; set; }
		public static IResourceService ResourceService { get; set; }
		public static IContentService ContentService { get; protected set; }
		public static IDragDropService DragDropService { get; protected set; }
		public static ISecurityService SecurityService { get; protected set; }
	}
}