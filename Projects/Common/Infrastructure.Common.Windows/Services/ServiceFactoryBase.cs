using Infrastructure.Common.Windows.Services.Content;
using Infrastructure.Common.Windows.Services.DragDrop;
using Infrastructure.Services;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Common.Windows.Services
{
	public abstract class ServiceFactoryBase
	{
		static ServiceFactoryBase()
		{
			Events = new EventAggregator();
			ResourceService = new ResourceService();
			DialogService = new RealDialogService();
			MessageBoxService = new RealMessageBoxService();
		}

		public static IEventAggregator Events { get; set; }
		public static IResourceService ResourceService { get; set; }
		public static IDialogService DialogService { get; set; }
		public static IMessageBoxService MessageBoxService { get; set; }
		public static IContentService ContentService { get; protected set; }
		public static IDragDropService DragDropService { get; protected set; }
		public static ISecurityService SecurityService { get; protected set; }
	}
}