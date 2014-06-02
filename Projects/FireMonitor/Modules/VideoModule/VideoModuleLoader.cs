using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
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

namespace VideoModule
{
	public class VideoModuleLoader : ModuleBase, ILayoutProviderModule
	{
		CamerasViewModel CamerasViewModel;
		NavigationItem _videoNavigationItem;
		PlanPresenter _planPresenter;

		public override void CreateViewModels()
		{
			_planPresenter = new PlanPresenter();
			CamerasViewModel = new CamerasViewModel();
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
			foreach (var camera in FiresecManager.SystemConfiguration.AllCameras)
			{
				foreach (var zoneUID in camera.ZoneUIDs)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.BaseUID == zoneUID);
					if (zone != null)
					{
						if (zone.State.StateClass == camera.StateClass)
						{
							DialogService.ShowWindow(new CameraDetailsViewModel(camera));
						}
					}
				}
			}
		}

		public override void Initialize()
		{
			_videoNavigationItem.IsVisible = FiresecManager.SystemConfiguration.AllCameras.Count > 0;
			CamerasViewModel.Initialize();
			_planPresenter.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(_planPresenter);
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_videoNavigationItem = new NavigationItem<ShowCameraEvent, Guid>(CamerasViewModel, "Видео", "/Controls;component/Images/Video1.png");
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
			yield return new LayoutPartPresenter(LayoutPartIdentities.CamerasList, "Список камер", "Video1.png", (p) => CamerasViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.CameraVideo, "Видео с камеры", "Video1.png", (p) => new LayoutPartCameraViewModel(p as LayoutPartCameraProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.MultiCamera, "Видео с камер", "Video1.png", (p) => new LayoutMultiCameraViewModel()); 
		}
		#endregion
	}
}