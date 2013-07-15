using Infrastructure.Common.Services.Content;
using Infrastructure.Common.Services.DragDrop;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Common.Services
{
	public abstract class ServiceFactoryBase
	{
		public static IEventAggregator Events { get; protected set; }
		public static IResourceService ResourceService { get; protected set; }
		public static IContentService ContentService { get; protected set; }
		public static IDragDropService DragDropService { get; protected set; }
	}
}
