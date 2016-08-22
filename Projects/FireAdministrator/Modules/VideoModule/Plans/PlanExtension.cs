using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Localization.Video.Common;
using Localization.Video.Errors;
using Infrastructure.Common.Services;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrustructure.Plans.Designer;
using StrazhAPI.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using VideoModule.Plans.Designer;
using VideoModule.Plans.ViewModels;
using VideoModule.ViewModels;
using Infrastructure.Events;

namespace VideoModule.Plans
{
	class PlanExtension : BasePlanExtension
	{
		public static PlanExtension Instance { get; private set; }
		private readonly CamerasViewModel _camerasViewModel;

		public PlanExtension(CamerasViewModel camerasViewModel)
		{
			Instance = this;
			ServiceFactoryBase.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactoryBase.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactoryBase.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactoryBase.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_camerasViewModel = camerasViewModel;
			Cache.Add(() => FiresecManager.SystemConfiguration.Cameras);
		}

		#region IPlanExtension Members

		public override int Index
		{
			get { return 1; }
		}
		public override string Title
		{
			get { return CommonResources.Videocam; }
		}

		public override IEnumerable<IInstrument> Instruments
		{
			get
			{
				return Enumerable.Empty<IInstrument>();
			}
		}

		public override bool ElementAdded(Plan plan, ElementBase element)
		{
			if (element is ElementCamera)
			{
				var elementCamera = element as ElementCamera;
				plan.ElementExtensions.Add(elementCamera);
				SetItem<Camera>(elementCamera);
				return true;
			}
			return false;
		}
		public override bool ElementRemoved(Plan plan, ElementBase element)
		{
			if (element is ElementCamera)
			{
				var elementCamera = (ElementCamera)element;
				plan.ElementExtensions.Remove(elementCamera);
				ResetItem<Camera>(elementCamera);
				return true;
			}
			return false;
		}

		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementCamera)
				RegisterDesignerItem<Camera>(designerItem, "CameraVideo", "/Controls;component/Images/BVideo.png");
		}

		public override IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			if (plan.ElementExtensions == null)
				plan.ElementExtensions = new List<ElementBase>();

			return plan.ElementExtensions.OfType<ElementCamera>();
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup("CameraVideo", CommonResources.Cameras, 41);
		}

		public override IEnumerable<ElementError> Validate()
		{
			var errors = new List<ElementError>();
			FiresecManager.PlansConfiguration.AllPlans.ForEach(plan =>
				errors.AddRange(FindUnbindedErrors<ElementCamera, ShowVideoEvent, Guid>(plan.ElementExtensions.OfType<ElementCamera>(), plan.UID, CommonErrors.UnrelatedCamera_Error, "/Controls;component/Images/Camera.png", Guid.Empty)));

			return errors;
		}

		#endregion

		protected override void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
		{
			if (typeof(TItem) == typeof(Camera))
			{
				var camera = item as Camera;
                designerItem.Title = camera == null ? CommonErrors.UnknownCamera_Error : camera.Name;
			}
			else
				base.UpdateDesignerItemProperties(designerItem, item);
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementCamera = args.Element as ElementCamera;
			if (elementCamera != null)
				args.Painter = new Painter(DesignerCanvas, elementCamera);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			var element = e.Element as ElementCamera;

			if (element != null)
				e.PropertyViewModel = new CameraPropertiesViewModel(_camerasViewModel, element);
		}
	}
}