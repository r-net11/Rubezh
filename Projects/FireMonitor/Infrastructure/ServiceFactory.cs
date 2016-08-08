using System;
using System.Windows;
using StrazhAPI.Models;
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
			get { return _startupService ?? (_startupService = new StartupService(ClientType.Monitor)); }
		}
		public static AppSettings AppSettings { get; set; }
		public static ILayoutService Layout { get; private set; }
		public static LoginService LoginService { get; private set; }

		/// <summary>
		/// Сервис, который определяет видимость элементов UI ОЗ, согласно данным лицензии
		/// </summary>
		public static IUiElementsVisibilityService UiElementsVisibilityService { get; private set; }

		public static void Initialize(ILayoutService layoutService, ISecurityService securityService, IUiElementsVisibilityService uiElementsVisibilityService)
		{
			Events = Events = new EventAggregator();
			SecurityService = securityService;
			ResourceService = new ResourceService();
			Layout = layoutService;
			LoginService = new LoginService(ClientType.Monitor, "Оперативная задача. Авторизация.");
			ContentService = new ContentService("Monitor");
			DragDropService = new DragDropService();
			UiElementsVisibilityService = uiElementsVisibilityService;
		}

		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}
	}
}