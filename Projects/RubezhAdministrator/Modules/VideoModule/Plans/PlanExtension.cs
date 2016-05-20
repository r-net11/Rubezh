using Common;
using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Plans.Designer;
using Infrastructure.Plans.Events;
using Infrastructure.Plans.Services;
using Infrastructuret.Plans;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using RubezhAPI.Plans.Interfaces;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using VideoModule.Plans.Designer;
using VideoModule.Plans.ViewModels;

namespace VideoModule.Plans
{
	class PlanExtension : BasePlanExtension
	{
		public static PlanExtension Instance { get; private set; }

		public PlanExtension()
		{
			Instance = this;
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			Cache.Add<Camera>(() => ClientManager.SystemConfiguration.Cameras);
		}

		#region IPlanExtension Members

		public override int Index
		{
			get { return 1; }
		}
		public override string Title
		{
			get { return "Видеокамера"; }
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
			foreach (var element in plan.ElementExtensions)
				if (element is ElementCamera)
					yield return element;
		}

		public override void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			base.ExtensionRegistered(designerCanvas);
			LayerGroupService.Instance.RegisterGroup("CameraVideo", "Камеры", 41);
		}
		public override void ExtensionAttached()
		{
			using (new TimeCounter("CamerasList.ExtensionAttached.BuildMap: {0}"))
				base.ExtensionAttached();
		}

		public override IEnumerable<ElementError> Validate()
		{
			List<ElementError> errors = new List<ElementError>();
			ClientManager.PlansConfiguration.AllPlans.ForEach(plan =>
				errors.AddRange(FindUnbindedErrors<ElementCamera, ShowVideoEvent, Guid>(plan.ElementExtensions.OfType<ElementCamera>(), plan.UID, "Несвязанная камера", "/Controls;component/Images/Camera.png", Guid.Empty)));
			return errors;
		}

		#endregion

		protected override void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
		{
			if (typeof(TItem) == typeof(Camera))
			{
				var camera = item as Camera;
				designerItem.Title = camera == null ? "Неизвестная камера" : camera.PresentationName;
				var vizualizationItem = designerItem.Element as IMultipleVizualization;
				if (camera != null && vizualizationItem != null)
				{
					vizualizationItem.AllowMultipleVizualization = camera.AllowMultipleVizualization;
				}
			}
			else
				base.UpdateDesignerItemProperties<TItem>(designerItem, item);
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementCamera = args.Element as ElementCamera;
			if (elementCamera != null)
				args.Painter = new Painter(DesignerCanvas, elementCamera);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementCamera element = e.Element as ElementCamera;
			if (element != null)
				e.PropertyViewModel = new CameraPropertiesViewModel(element, DesignerCanvas);
		}
	}
}