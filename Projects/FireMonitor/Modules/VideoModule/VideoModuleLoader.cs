using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.License;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using VideoModule.Plans;
using VideoModule.ViewModels;

namespace VideoModule
{
	public partial class VideoModuleLoader : ModuleBase, ILayoutProviderModule
	{
		CamerasViewModel CamerasViewModel;
		NavigationItem _videoNavigationItem;
		PlanPresenter _planPresenter;

		public override void CreateViewModels()
		{
			_planPresenter = new PlanPresenter();
			CamerasViewModel = new CamerasViewModel();

			SubscribeShowDelailsEvent();
		}

		#region ShowDelailsEvent
		void SubscribeShowDelailsEvent()
		{
			ServiceFactory.Events.GetEvent<ShowCameraDetailsEvent>().Unsubscribe(OnShowDeviceDetails);
			ServiceFactory.Events.GetEvent<ShowCameraDetailsEvent>().Subscribe(OnShowDeviceDetails);
		}

		void OnShowDeviceDetails(Guid videoUID)
		{
			var videoDevice = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == videoUID);
			if (videoDevice != null)
			{
				DialogService.ShowWindow(new CameraDetailsViewModel(videoDevice));
			}
		}
		#endregion

		public override void Initialize()
		{
			_videoNavigationItem.IsVisible = LicenseManager.CurrentLicenseInfo.HasVideo && ClientManager.SystemConfiguration.Cameras.Count > 0;
			CamerasViewModel.Initialize();
			_planPresenter.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanPresenterEvent<Plan, XStateClass>>().Publish(_planPresenter);
		}

		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			_videoNavigationItem = new NavigationItem<ShowCameraEvent, Guid>(CamerasViewModel, ModuleType.ToDescription(), "Video1");
			return new List<NavigationItem>()
			{
				_videoNavigationItem
			};
		}

		public override ModuleType ModuleType
		{
			get { return ModuleType.Video; }
		}
		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "DataTemplates/Dictionary.xaml");
		}

		#region ILayoutProviderModule Members
		public IEnumerable<ILayoutPartPresenter> GetLayoutParts()
		{
			yield return new LayoutPartPresenter(LayoutPartIdentities.CamerasList, "Список камер", "Video1.png", (p) => CamerasViewModel);
			yield return new LayoutPartPresenter(LayoutPartIdentities.CameraVideo, "Одна камера", "Video1.png", (p) => new LayoutPartCameraViewModel(p as LayoutPartReferenceProperties));
			yield return new LayoutPartPresenter(LayoutPartIdentities.MultiCamera, "Раскладка камер", "Video1.png", (p) => new LayoutMultiCameraViewModel());
		}
		#endregion
		public override void AfterInitialize()
		{
			RviAfterInitialize();
		}
	}
}