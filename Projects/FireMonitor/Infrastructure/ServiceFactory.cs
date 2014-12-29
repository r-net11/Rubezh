using System;
using System.Collections.Generic;
using System.Windows;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Client.Login;
using Infrastructure.Client.Startup;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Content;
using Infrastructure.Common.Services.DragDrop;
using Infrastructure.Events;
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

		public static void Initialize(ILayoutService ILayoutService, ISecurityService ISecurityService)
		{
			ServiceFactoryBase.Events = Events = new EventAggregator();
			ServiceFactoryBase.SecurityService = SecurityService = ISecurityService;
			ResourceService = new ResourceService();
			Layout = ILayoutService;
			LoginService = new LoginService(ClientType.Monitor, "Оперативная задача. Авторизация.");
			ContentService = new ContentService("Monitor");
			DragDropService = new DragDropService();
		}

		static void OnDeviceStateChangedEvent(List<DeviceState> deviceStates)
		{
			foreach (var deviceState in deviceStates)
			{
				if (deviceState != null)
				{
					deviceState.OnStateChanged();
				}
			}
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Publish(null);
		}
		static void OnDeviceParametersChangedEvent(List<DeviceState> deviceStates)
		{
			foreach (var deviceState in deviceStates)
			{
				ServiceFactory.Events.GetEvent<DeviceParametersChangedEvent>().Publish(deviceState.Device.UID);
				if (deviceState != null)
				{
					deviceState.OnParametersChanged();
				}
			}
		}
		static void OnZoneStateChangedEvent(List<ZoneState> zoneStates)
		{
			foreach (var zoneState in zoneStates)
			{
				ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Publish(zoneState.Zone.UID);
				if (zoneState != null)
				{
					zoneState.OnStateChanged();
				}
			}
		}

		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}
	}
}