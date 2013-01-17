using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Elements;
using FiresecAPI.Models;
using Infrustructure.Plans.Painters;
using System.Windows.Media;
using Infrastructure;
using Infrustructure.Plans.Presenter;

namespace PlansModule.Designer
{
	class PresenterCanvas : CommonDesignerCanvas
	{
		private double _zoom;
		private double _pointZoom;
		public Plan Plan { get; private set; }
		public override double Zoom
		{
			get { return _zoom; }
		}
		public override double PointZoom
		{
			get { return _pointZoom; }
		}

		public PresenterCanvas()
			: base(ServiceFactory.Events)
		{
			_zoom = base.Zoom;
			_pointZoom = base.PointZoom;
		}

		public override void BeginChange(IEnumerable<DesignerItem> designerItems)
		{
		}
		public override void BeginChange()
		{
		}
		public override void EndChange()
		{
		}
		public override void Remove(List<Guid> elementUIDs)
		{
		}
		public override void CreateDesignerItem(ElementBase element)
		{
		}

		public IEnumerable<PresenterItem> PresenterItems
		{
			get { return InternalItems<PresenterItem>(); }
		}

		public override void Update()
		{
			Update(Plan);
		}
		private void Update(Plan plan)
		{
			CanvasWidth = plan.Width;
			CanvasHeight = plan.Height;
			if (plan.BackgroundPixels != null)
				CanvasBackground = PainterHelper.CreateBrush(plan.BackgroundPixels);
			else if (plan.BackgroundColor == Colors.Transparent)
				CanvasBackground = PainterHelper.CreateTransparentBrush(Zoom);
			else
				CanvasBackground = new SolidColorBrush(plan.BackgroundColor);
		}

		public void ShowPlan(Plan plan)
		{
			Plan = plan;
			ShowCanvas(plan == null ? Guid.Empty : plan.UID);
		}
		public void RegisterPlan(Plan plan)
		{
			AddCanvas(plan.UID);
			Update(plan);
			SelectedCanvas.Visibility = System.Windows.Visibility.Collapsed;
		}

		public void UpdateZoom(double zoom, double deviceZoom)
		{
			ZoomChanged(zoom, deviceZoom);
			foreach (var presenterItem in PresenterItems)
			{
				presenterItem.UpdateZoom();
				if (presenterItem.IsPoint)
					presenterItem.UpdateZoomPoint();
			}
		}
		public void UpdateZoomPoint(double zoom, double deviceZoom)
		{
			ZoomChanged(zoom, deviceZoom);
			foreach (var presenterItem in PresenterItems)
				if (presenterItem.IsPoint)
					presenterItem.UpdateZoomPoint();
		}
		private void ZoomChanged(double zoom, double deviceZoom)
		{
			_zoom = zoom;
			_pointZoom = deviceZoom / zoom;
			ZoomChanged();
		}
	}
}
