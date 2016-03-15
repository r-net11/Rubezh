using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Enums;
using FiresecAPI.Models;
using FiresecAPI.Models.Layouts;
using Infrastructure;
using Infrastructure.Client;
using Infrastructure.Client.Layout;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using Infrastructure.Common.Services.Layout;
using Infrastructure.Common.Validation;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
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
			CamerasViewModel = new CamerasViewModel();
			_planExtension = new PlanExtension(CamerasViewModel);
		}

		public override void Initialize()
		{
			CamerasViewModel.Initialize();
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
			ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Plans/DataTemplates/Dictionary.xaml"));
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
			yield return new LayoutPartDescription(LayoutPartDescriptionGroup.Video, LayoutPartIdentities.CameraVideo, 204, "Камера", "Панель видео с камеры", "BVideo.png")
			{
				Factory = (p) => new LayoutPartCameraViewModel(p as LayoutPartReferenceProperties),
			};
		}
	}
}