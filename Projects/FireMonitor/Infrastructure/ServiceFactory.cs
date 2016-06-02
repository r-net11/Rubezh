using System;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Client.Login;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Content;
using Infrastructure.Common.Services.DragDrop;
using Microsoft.Practices.Prism.Events;

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
		public static AppSettings AppSettings { get; set; }
		public static ILayoutService Layout { get; private set; }
		public static LoginService LoginService { get; private set; }

		/// <summary>
		/// Сервис, который определяет видимость элементов UI ОЗ, согласно данным лицензии
		/// </summary>
	//	public static IUiElementsVisibilityService UiElementsVisibilityService { get; private set; }

		//public static void Initialize(ILayoutService ILayoutService, ISecurityService ISecurityService, IUiElementsVisibilityService uiElementsVisibilityService)
		public static void Initialize(ILayoutService ILayoutService, ISecurityService ISecurityService)
		{
			ServiceFactoryBase.Events = Events = new EventAggregator();
			ServiceFactoryBase.SecurityService = SecurityService = ISecurityService;
			ResourceService = new ResourceService();
			Layout = ILayoutService;
			LoginService = new LoginService(ClientType.Monitor, "Оперативная задача. Авторизация.");
			ContentService = new ContentService("Monitor");
			DragDropService = new DragDropService();
		//	UiElementsVisibilityService = uiElementsVisibilityService;
		}

		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}
	}
}