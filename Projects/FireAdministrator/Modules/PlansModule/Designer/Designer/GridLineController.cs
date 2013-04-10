using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;

namespace PlansModule.Designer
{
	public class GridLineController : IGridLineController
	{
		private const double DELTA = 20;
		private DesignerCanvas _canvas;
		private StreamGeometry _geometry;
		private RectangleGeometry _clipGeometry;
		private bool _isVisible;
		public ObservableCollection<GridLine> GridLines { get; private set; }
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = false;
#if DEBUG
				_isVisible = value;
#endif
				if (_canvas != null)
					_canvas.Refresh();
			}
		}

		public GridLineController(DesignerCanvas canvas)
		{
			IsVisible = true;
			_canvas = canvas;
			_geometry = new StreamGeometry();
			GridLines = new ObservableCollection<GridLine>();
			GridLines.CollectionChanged += (s, e) => Invalidate();
			Invalidate();
		}
		public void Invalidate()
		{
			_geometry = new StreamGeometry();
			using (StreamGeometryContext ctx = _geometry.Open())
				foreach (var gridLine in GridLines)
				{
					ctx.BeginFigure(gridLine.Orientation == Orientation.Horizontal ? new Point(0, gridLine.Position) : new Point(gridLine.Position, 0), false, false);
					ctx.LineTo(gridLine.Orientation == Orientation.Horizontal ? new Point(_canvas.Width, gridLine.Position) : new Point(gridLine.Position, _canvas.Height), true, false);
				}
			_geometry.Freeze();
			_clipGeometry = new RectangleGeometry(new Rect(0, 0, _canvas.CanvasWidth, _canvas.CanvasHeight));
			_canvas.Refresh();
		}
		public void Render(DrawingContext drawingContext)
		{
			if (IsVisible)
			{
				drawingContext.PushClip(_clipGeometry);
				drawingContext.DrawGeometry(null, PainterCache.GridLinePen, _geometry);
				drawingContext.Pop();
			}
		}

		public Vector PullRectangle(Vector shift, Rect rect)
		{
			var factor = DELTA / _canvas.Zoom;
			if (IsVisible)
				foreach (var gridLine in GridLines)
					switch (gridLine.Orientation)
					{
						case Orientation.Vertical:
							if (Math.Abs(rect.Left - gridLine.Position) < factor)
								shift.X = gridLine.Position - rect.Left;
							else if (Math.Abs(rect.Right - gridLine.Position) < factor)
								shift.X = gridLine.Position - rect.Right;
							break;
						case Orientation.Horizontal:
							if (Math.Abs(rect.Top - gridLine.Position) < factor)
								shift.Y = gridLine.Position - rect.Top;
							else if (Math.Abs(rect.Bottom - gridLine.Position) < factor)
								shift.Y = gridLine.Position - rect.Bottom;
							break;
					}
			return shift;
		}
	}
}
