using Infrastructure.Plans.Painters;
using System.Windows.Media;

namespace Infrastructure.Plans.Presenter
{
	public class BorderPainter
	{
		private ScaleTransform _transform;
		private PathGeometry _geometry;
		private Pen _pen;
		private PresenterItem _presenterItem;

		public BorderPainter()
		{
			var brush = new SolidColorBrush(Colors.Orange);
			brush.Freeze();
			_pen = new Pen(brush, 3);
			_transform = new ScaleTransform(0, 0);
			_geometry = new PathGeometry();
			_geometry.FillRule = FillRule.EvenOdd;
			var figure = new PathFigure();
			var polyline = new PolyLineSegment();
			polyline.IsStroked = true;
			figure.Segments.Add(polyline);
			figure.IsClosed = true;
			_geometry.Figures.Add(figure);
		}
		public void Render(DrawingContext drawingContext)
		{
			drawingContext.PushTransform(_transform);
			drawingContext.DrawGeometry(null, _pen, _geometry);
			drawingContext.Pop();
		}
		public void UpdateZoom(double zoom)
		{
			_pen.Thickness = 3 / zoom;
			UpdateBounds();
		}
		public void Hide()
		{
			_presenterItem = null;
			_transform.ScaleX = 0;
			_transform.ScaleY = 0;
		}
		public void Show(PresenterItem presenterItem)
		{
			_presenterItem = presenterItem;
			UpdateBounds();
			_transform.ScaleX = 1;
			_transform.ScaleY = 1;
		}

		private void UpdateBounds()
		{
			if (_presenterItem != null)
			{
				var points = _presenterItem.IsPoint ? PainterHelper.GetPoints(_presenterItem.GetRectangle(), _pen.Thickness / 2) : PainterHelper.GetRealPoints(_presenterItem.Element);
				if (points.Count > 0)
				{
					var figure = _geometry.Figures[0];
					figure.StartPoint = points[0];
					var polyLine = (PolyLineSegment)figure.Segments[0];
					polyLine.Points = points.ToWindowsPointCollection();
				}
			}
		}
	}
}