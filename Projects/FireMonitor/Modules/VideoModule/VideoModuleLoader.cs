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
using Infrastructure.Common.Services.Layout;
using Infrastructure.Client.Layout;

namespace VideoModule
{
	public class VideoModuleLoader : ModuleBase, ILayoutProviderModule
	{
		VideoViewModel VideoViewModel;
		NavigationItem _videoNavigationItem;

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
						if (zone.State.StateClass == camera.StateClass)
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
			_videoNavigationItem.IsVisible = FiresecManager.SystemConfiguration.Cameras.Count > 0;
			VideoViewModel.Initialize();
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_videoNavigationItem = new NavigationItem<ShowVideoEvent>(VideoViewModel, "Видео", "/Controls;component/Images/Video1.png");
			return new List<NavigationItem>()
		    {
		        _videoNavigationItem
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

		#region ILayoutProviderModule Members

		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter()
			{
				Name = "Видео",
				UID = LayoutPartIdentities.Video,
				IconSource = "/Controls;component/Images/Video1.png",
				Factory = (p) => VideoViewModel,
			};
		}

		#endregion
	}
}