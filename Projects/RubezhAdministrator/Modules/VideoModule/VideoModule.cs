using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrastructure.Events;
using Infrastructure.Plans.Events;
using RubezhAPI;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using VideoModule.Plans;
using VideoModule.Validation;
using VideoModule.ViewModels;
using CamerasViewModel = VideoModule.ViewModels.CamerasViewModel;

namespace VideoModule
{
	public class VideoModule : ModuleBase, IValidationModule, ILayoutDeclarationModule
	{
		CamerasViewModel CamerasViewModel;
		PlanExtension _planExtension;

		public override void CreateViewModels()
		{
			ServiceFactory.Events.GetEvent<SelectCameraEvent>().Subscribe(OnSelectCamera);
			ServiceFactory.Events.GetEvent<SelectCamerasEvent>().Subscribe(OnSelectCameras);

			CamerasViewModel = new CamerasViewModel();
			_planExtension = new PlanExtension();
		}

		public override void Initialize()
		{
			CamerasViewModel.Initialize();
		}

		public override void RegisterPlanExtension()
		{
			_planExtension.Initialize();
			ServiceFactory.Events.GetEvent<RegisterPlanExtensionEvent<Plan>>().Publish(_planExtension);
			_planExtension.Cache.BuildAllSafe();
		}

		public IEnumerable<IValidationError> Validate()
		{
			var validator = new Validator();
			return validator.Validate();
		}

		public override void RegisterResource()
		{
			base.RegisterResource();
			ServiceFactory.ResourceService.AddResource(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml");
		}
		public override IEnumerable<NavigationItem> CreateNavigation()
		{
			return new List<NavigationItem>()
			{
				new NavigationItem<ShowVideoEvent, Guid>(CamerasViewModel,ModuleType.ToDescription(), "Video1"),
			};
		}
		public override ModuleType ModuleType
		{
			get { return ModuleType.Video; }
		}

		public IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions()
		{
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Video, LayoutPartIdentities.CamerasList, 203, "Список камер", "Панель список камер", "BVideo.png", false);
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Video, LayoutPartIdentities.CameraVideo, 204, "Одна камера", "Панель видео с камеры", "BVideo.png")
			{
				Factory = (p) => new LayoutPartCameraViewModel(p as LayoutPartReferenceProperties),
			};
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Video, LayoutPartIdentities.MultiCamera, 205, "Раскладка камер", "Панель раскладки камер", "BVideo.png", false);
		}

		void OnSelectCamera(SelectCameraEventArg selectCameraEventArg)
		{
			var cameraSelectionViewModel = new CameraSelectionViewModel(selectCameraEventArg.Camera);
			selectCameraEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(cameraSelectionViewModel);
			selectCameraEventArg.Camera = selectCameraEventArg.Cancel || cameraSelectionViewModel.SelectedCamera == null ?
				null :
				cameraSelectionViewModel.SelectedCamera.Camera;
		}
		void OnSelectCameras(SelectCamerasEventArg selectCamerasEventArg)
		{
			var camerasSelectionViewModel = new CamerasSelectionViewModel(selectCamerasEventArg.Cameras);
			selectCamerasEventArg.Cancel = !ServiceFactory.DialogService.ShowModalWindow(camerasSelectionViewModel);
			selectCamerasEventArg.Cameras = camerasSelectionViewModel.TargetCameras.ToList();
		}
	}
}