using System.Collections.Generic;
using Infrastructure.Client;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using VideoModule.ViewModels;
using FiresecClient;
using System;
using System.Linq;
using Infrastructure;

namespace VideoModule
{
	public class VideoModuleLoader : ModuleBase
	{
		VideoViewModel VideoViewModel;

		public override void CreateViewModels()
		{
			VideoViewModel = new VideoViewModel();
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Subscribe(OnDevicesStateChanged);
		}

		void OnDevicesStateChanged(object obj)
		{
			UpdateVideoAlarms();
		}

		void UpdateVideoAlarms()
		{
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				foreach (var zoneUID in camera.ZoneUIDs)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						if (zone.ZoneState.StateClass == camera.StateClass)
						{
							VideoService.Show(camera);
						}
					}
				}
			}
		}

		public override void Initialize()
		{
			OnDevicesStateChanged(Guid.Empty);
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
		    {
		        new NavigationItem<ShowVideoEvent>(VideoViewModel, "Видео", "/Controls;component/Images/Video1.png"),
		    };
		}

		public override string Name
		{
			get { return "Видео"; }
		}

		public override void Dispose()
		{
			VideoService.Close();
		}
	}
}