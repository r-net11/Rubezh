using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using VideoModule.Plans;
using VideoModule.ViewModels;
using XFiresecAPI;

namespace VideoModule
{
	public class VideoModuleLoader : ModuleBase, ILayoutProviderModule
	{
		AllVideoViewModel AllVideoViewModel;
		//CamerasViewModel _CamerasViewModel;
		NavigationItem _videoNavigationItem;
		PlanPresenter _planPresenter;

		public override void CreateViewModels()
		{
			_planPresenter = new PlanPresenter();
			//_CamerasViewModel = new CamerasViewModel();
			AllVideoViewModel = new AllVideoViewModel();
			foreach (var zone in XManager.Zones)
			{
				zone.State.StateChanged -= new Action(OnZoneStateChanged);
				zone.State.StateChanged += new Action(OnZoneStateChanged);
			}
		}

		void OnZoneStateChanged()
		{
			UpdateVideoAlarms();
		}

		void UpdateVideoAlarms()
		{
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				foreach (var zoneUID in camera.ZoneUIDs)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
					if (zone != null)
					{
						if (zone.State.StateClass == camera.StateClass)
						{
							DialogService.ShowWindow(new CameraDetailsViewModel(camera));
							//VideoService.Show(camera);
						}
					}
				}
			}
		}

		public override void Initialize()
		{
			//UpdateVideoAlarms();
			_videoNavigationItem.IsVisible = FiresecManager.SystemConfiguration.Cameras.Count > 0;
			//_CamerasViewModel.Initialize();
			AllVideoViewModel.Initialize();
			_planPresenter.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(_planPresenter);
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			//_videoNavigationItem = new NavigationItem<ShowCameraEvent, Guid>(_CamerasViewModel, "Видео", "/Controls;component/Images/Video1.png");
			_videoNavigationItem = new NavigationItem<ShowCameraEvent, Guid>(AllVideoViewModel, "Видео", "/Controls;component/Images/Video1.png");
			return new List<NavigationItem>()
			{
				_videoNavigationItem
			};
		}

		public override string Name
		{
			get { return "Видео"; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
		}
		public override void Dispose()
		{
			VideoService.Close();
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.CamerasList, "Список камер", "Video1.png", (p) => new CamerasViewModel());
			yield return new LayoutPartPresenter(LayoutPartIdentities.CameraVideo, "Видео с камеры", "Video1.png", (p) => new LayoutPartCameraViewModel(p as LayoutPartCameraProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.MultiCamera, "Видео с камер", "Video1.png", (p) => new LayoutMultiCameraViewModel()); 
		}
		#endregion
	}
}