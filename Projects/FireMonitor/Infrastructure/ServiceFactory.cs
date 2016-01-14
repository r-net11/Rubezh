using System;
using System.Windows;
using RubezhAPI.Models;
using Infrastructure.Client.Login;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Content;
using Infrastructure.Common.Services.DragDrop;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Services;

namespace Infrastructure
{
	public class ServiceFactory : ServiceFactoryBase
	{
		private static StartupService _startupService;
		public static StartupService StartupService
		{
			get
			{
				if (_startupService == null)
					_startupService = new StartupService(ClientType.Monitor);
				return _startupService;
			}
		}
		public static ILayoutService Layout { get; private set; }

		public static void Initialize(ILayoutService layoutService, ISecurityService securityService)
		{
			ServiceFactoryBase.SecurityService = SecurityService = securityService;
			Layout = layoutService;
			ContentService = new ContentService("Monitor");
			DragDropService = new DragDropService();
		}
	}
}