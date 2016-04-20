using Infrastructure.Client.Startup;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.Services.Content;
using Infrastructure.Common.Windows.Services.DragDrop;
using Infrastructure.Common.Windows.Services.Ribbon;
using Infrastructure.Services;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.Models;

namespace Infrastructure
{
	public class ServiceFactory : ServiceFactoryBase
	{
		static StartupService _startupService;
		public static StartupService StartupService
		{
			get
			{
				if (_startupService == null)
					_startupService = new StartupService(ClientType.Administrator);
				return _startupService;
			}
		}
		
		public static void Initialize(ILayoutService layoutService, IValidationService validationService)
		{
			SaveService = new SaveService();
			Layout = layoutService;
			ValidationService = validationService;
			ContentService = new ContentService("Administrator");
			DragDropService = new DragDropService();
			RibbonService = new RibbonService();
		}

		public static SaveService SaveService { get; private set; }
		public static ILayoutService Layout { get; private set; }
		public static IValidationService ValidationService { get; private set; }
		public static MenuService MenuService { get; set; }
		public static IRibbonService RibbonService { get; set; }
	}
}