﻿using FiresecAPI.Models;
using Infrastructure.Client.Login;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Content;
using Infrastructure.Common.Services.DragDrop;
using Infrastructure.Common.Services.Ribbon;
using Infrastructure.Services;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Client.Startup;

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
		
		public static void Initialize(ILayoutService ILayoutService, IValidationService IValidationService)
		{
			SaveService = new SaveService();
			Events = new EventAggregator();
			ResourceService = new ResourceService();
			Layout = ILayoutService;
			ValidationService = IValidationService;
			LoginService = new LoginService(ClientType.Administrator, "Администратор. Авторизация");
			ContentService = new ContentService("Administrator");
			DragDropService = new DragDropService();
			RibbonService = new RibbonService();
		}

		public static SaveService SaveService { get; private set; }
		public static ILayoutService Layout { get; private set; }
		public static IValidationService ValidationService { get; private set; }
		public static LoginService LoginService { get; private set; }
		public static MenuService MenuService { get; set; }
		public static IRibbonService RibbonService { get; private set; }
	}
}