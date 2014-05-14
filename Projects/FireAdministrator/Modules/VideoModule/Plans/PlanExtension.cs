using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using Infrastructure;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using Infrustructure.Plans.Services;
using VideoModule.Plans.Designer;
using VideoModule.Plans.ViewModels;
using VideoModule.ViewModels;

namespace VideoModule.Plans
{
	class PlanExtension : IPlanExtension<Plan>
	{
		private CamerasViewModel _camerasViewModel;
		private CommonDesignerCanvas _designerCanvas;

		public PlanExtension(CamerasViewModel camerasViewModel)
		{
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Unsubscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<PainterFactoryEvent>().Subscribe(OnPainterFactoryEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Unsubscribe(OnShowPropertiesEvent);
			ServiceFactory.Events.GetEvent<ShowPropertiesEvent>().Subscribe(OnShowPropertiesEvent);

			_camerasViewModel = camerasViewModel;
		}

		public void Initialize()
		{
		}

		#region IPlanExtension Members

		public string Title
		{
			get { return "Видеокамера"; }
		}

		public IEnumerable<IInstrument> Instruments
		{
			get
			{
				return Enumerable.Empty<IInstrument>();
			}
		}

		public bool ElementAdded(Plan plan, ElementBase element)
		{
			if (element is ElementCamera)
			{
				var elementCamera = element as ElementCamera;
				Helper.SetCamera(elementCamera);
				plan.ElementExtensions.Add(elementCamera);
				return true;
			}
			return false;
		}
		public bool ElementRemoved(Plan plan, ElementBase element)
		{
			if (element is ElementCamera)
			{
				var elementCamera = (ElementCamera)element;
				plan.ElementExtensions.Remove(elementCamera);
				return true;
			}
			return false;
		}

		public void RegisterDesignerItem(DesignerItem designerItem)
		{
			if (designerItem.Element is ElementCamera)
			{
				designerItem.ItemPropertyChanged += CameraPropertyChanged;
				OnCameraPropertyChanged(designerItem);
				designerItem.Group = "CameraVideo";
				designerItem.UpdateProperties += UpdateDesignerItemCamera;
				UpdateDesignerItemCamera(designerItem);
			}
		}

		public IEnumerable<ElementBase> LoadPlan(Plan plan)
		{
			if (plan.ElementExtensions == null)
				plan.ElementExtensions = new List<ElementBase>();
			foreach (var element in plan.ElementExtensions)
				if (element is ElementCamera)
					yield return element;
		}

		public void ExtensionRegistered(CommonDesignerCanvas designerCanvas)
		{
			_designerCanvas = designerCanvas;
			LayerGroupService.Instance.RegisterGroup("CameraVideo", "Камеры", 7);
		}
		public void ExtensionAttached()
		{
			using (new TimeCounter("CamerasList.ExtensionAttached.BuildMap: {0}"))
				Helper.BuildMap();
		}

		#endregion

		private void UpdateDesignerItemCamera(CommonDesignerItem designerItem)
		{
			ElementCamera element = designerItem.Element as ElementCamera;
			var camera = Helper.GetCamera(element);
			Helper.SetCamera(element, camera);
			designerItem.Title = Helper.GetCameraTitle(element);
			designerItem.IconSource = "/Controls;component/Images/BVideo.png";
		}

		private void CameraPropertyChanged(object sender, EventArgs e)
		{
			DesignerItem designerItem = (DesignerItem)sender;
			OnCameraPropertyChanged(designerItem);
		}
		private void OnCameraPropertyChanged(DesignerItem designerItem)
		{
			var camera = Helper.GetCamera((ElementCamera)designerItem.Element);
			if (camera != null)
				camera.Changed += () =>
				{
					if (_designerCanvas.IsPresented(designerItem))
					{
						Helper.BuildCameraMap();
						UpdateDesignerItemCamera(designerItem);
						designerItem.Painter.Invalidate();
						_designerCanvas.Refresh();
					}
				};
		}

		private void OnPainterFactoryEvent(PainterFactoryEventArgs args)
		{
			var elementCamera = args.Element as ElementCamera;
			if (elementCamera != null)
				args.Painter = new Painter(_designerCanvas, elementCamera);
		}
		private void OnShowPropertiesEvent(ShowPropertiesEventArgs e)
		{
			ElementCamera element = e.Element as ElementCamera;
			if (element != null)
				e.PropertyViewModel = new CameraPropertiesViewModel(_camerasViewModel, element);
		}
	}
}