using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Infrustructure.Plans.Designer;
using Infrustructure.Plans.Painters;

namespace Infrastructure.Designer
{
	public class GridLineController : IGridLineController
	{
		private const double DELTA = 20;
		private double _accamulateX = 0;
		private double _accamulateY = 0;
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
				_isVisible = value;
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
				drawingContext.DrawGeometry(null, _canvas.PainterCache.GridLinePen, _geometry);
				drawingContext.Pop();
			}
		}

		public void PullReset()
		{
			_accamulateX = 0;
			_accamulateY = 0;
		}
		public Vector Pull(Point point)
		{
			return Pull(new Rect(point, point));
		}
		public Vector Pull(Rect rect)
		{
			var shift = Pull(new Vector(0, 0), rect);
			PullReset();
			return shift;
		}
		public Vector Pull(Vector shift, Rect rect)
		{
			var factor = DELTA / _canvas.Zoom;
			if (IsVisible)
				foreach (var gridLine in GridLines)
					switch (gridLine.Orientation)
					{
						case Orientation.Vertical:
							shift.X = Pull(shift.X, gridLine.Position - rect.Left, factor, ref _accamulateX);
							if (rect.Left != rect.Right)
								shift.X = Pull(shift.X, gridLine.Position - rect.Right, factor, ref _accamulateX);
							break;
						case Orientation.Horizontal:
							shift.Y = Pull(shift.Y, gridLine.Position - rect.Top, factor, ref _accamulateY);
							if (rect.Top != rect.Bottom)
								shift.Y = Pull(shift.Y, gridLine.Position - rect.Bottom, factor, ref _accamulateY);
							break;
					}
			return shift;
		}

		private double Pull(double shift, double margin, double factor, ref double accamulate)
		{
			double result = shift;
			if (Math.Abs(margin) < factor)
			{
				accamulate += shift;
				if (Math.Abs(accamulate) < factor)
					result = margin;
				else
				{
					result = accamulate;
					accamulate = 0;
				}
			}
			return result;
		}
	}
}
