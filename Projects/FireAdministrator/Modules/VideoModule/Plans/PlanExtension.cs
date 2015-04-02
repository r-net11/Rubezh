﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Client.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using VideoModule.Plans.Designer;
using VideoModule.Plans.ViewModels;
using VideoModule.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Interfaces;

namespace VideoModule.Plans
{
	class PlanExtension : BasePlanExtension
	{
		public static PlanExtension Instance { get; private set; }
		private CamerasViewModel _camerasViewModel;

		public PlanExtension(CamerasViewModel camerasViewModel)
		{
			Instance = this;
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_camerasViewModel = camerasViewModel;
			Cache.Add<Camera>(() => FiresecManager.SystemConfiguration.Cameras);
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
			FiresecManager.PlansConfiguration.AllPlans.ForEach(plan =>
				errors.AddRange(FindUnbindedErrors<ElementCamera, ShowVideoEvent, Guid>(plan.ElementExtensions.OfType<ElementCamera>(), plan.UID, "Несвязанная камера", "/Controls;component/Images/Camera.png", Guid.Empty)));
			return errors;
		}

		#endregion

		protected override void UpdateDesignerItemProperties<TItem>(CommonDesignerItem designerItem, TItem item)
		{
			if (typeof(TItem) == typeof(Camera))
			{
				var camera = item as Camera;
				designerItem.Title = camera == null ? "Неизвестная камера" : camera.Name;
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
				e.PropertyViewModel = new CameraPropertiesViewModel(_camerasViewModel, element);
		}
	}
}