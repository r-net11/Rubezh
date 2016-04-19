using System.Collections.Generic;
using System.Windows.Media;
using RubezhAPI.Models;
using Infrastructure;
using Infrustructure.Plans.Designer;
using RubezhAPI.Plans.Elements;
using Infrustructure.Plans.Painters;
using Infrustructure.Plans.Presenter;

namespace PlansModule.Designer
{
	class PresenterCanvas : CommonDesignerCanvas
	{
		private double _zoom;
		private double _pointZoom;
		private BorderPainter _border;
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
			PainterCache.Initialize(ServiceFactory.ContentService.GetBitmapContent, ServiceFactory.ContentService.GetDrawing, ServiceFactory.ContentService.GetVisual);
			_zoom = base.Zoom;
			_pointZoom = base.PointZoom;
			_border = new BorderPainter();
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

		public override void CreateDesignerItem(ElementBase element)
		{
			CreatePresenterItem(element);
		}
		public PresenterItem CreatePresenterItem(ElementBase elementBase)
		{
			var presenterItem = new PresenterItem(elementBase);
			Add(presenterItem);
			return presenterItem;
		}
		public MonitorSubPlanPresenterItem CreateMonitorPresenterItem(ElementRectangleSubPlan elementBase)
		{
			var presenterItem = new MonitorSubPlanPresenterItem(elementBase);
			Add(presenterItem);
			return presenterItem;
		}

		public MonitorSubPlanPresenterItem CreateMonitorPresenterItem(ElementPolygonSubPlan elementBase)
		{
			var presenterItem = new MonitorSubPlanPresenterItem(elementBase);
			Add(presenterItem);
			return presenterItem;
		}

		public MonitorTextBoxPresenterItem CreateMonitorTextBoxPresenterItem(ElementTextBox elementBase)
		{
			var presenterItem = new MonitorTextBoxPresenterItem(elementBase);
			Add(presenterItem);
			return presenterItem;
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
			CanvasBackground = PainterCache.GetBrush(plan);
		}

		public void Initialize(Plan plan)
		{
			Plan = plan;
			Initialize();
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
			_border.UpdateZoom(Zoom);
		}

		protected override void RenderForeground(DrawingContext drawingContext)
		{
			base.RenderForeground(drawingContext);
			_border.Render(drawingContext);
		}
		protected override void SetDesignerItemOver(CommonDesignerItem designerItem, bool isOver)
		{
			base.SetDesignerItemOver(designerItem, isOver);
			if (isOver)
			{
				var presenterItem = designerItem as PresenterItem;
				if (presenterItem != null && presenterItem.ShowBorderOnMouseOver)
					_border.Show(presenterItem);
			}
			else
				_border.Hide();
		}
	}
}