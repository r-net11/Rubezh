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
			if (IsVisible)
			{
				var factor = DELTA / _canvas.Zoom;
				var deltaX = factor;
				var deltaY = factor;
				var positionX = 0.0;
				var positionY = 0.0;
				var gridLinePositionX = 0.0;
				var gridLinePositionY = 0.0;
				foreach (var gridLine in GridLines)
					switch (gridLine.Orientation)
					{
						case Orientation.Vertical:
							CalculateMinimum(rect.Right, shift.X, gridLine.Position, ref deltaX, ref positionX, ref gridLinePositionX);
							CalculateMinimum(rect.Left, shift.X, gridLine.Position, ref deltaX, ref positionX, ref gridLinePositionX);
							break;
						case Orientation.Horizontal:
							CalculateMinimum(rect.Top, shift.Y, gridLine.Position, ref deltaY, ref positionY, ref gridLinePositionY);
							CalculateMinimum(rect.Bottom, shift.Y, gridLine.Position, ref deltaY, ref positionY, ref gridLinePositionY);
							break;
					}
				var secondPass = false;
				shift.X = Pull(shift.X, factor, deltaX, positionX, gridLinePositionX, ref _accamulateX, ref secondPass);
				shift.Y = Pull(shift.Y, factor, deltaY, positionY, gridLinePositionY, ref _accamulateY, ref secondPass);
				if (secondPass)
					shift = Pull(shift, rect);
			}
			return shift;
		}

		private void CalculateMinimum(double border, double shift, double gridLine, ref double delta, ref double position, ref double gridLinePosition)
		{
			if (Math.Abs(border + shift - gridLine) < delta)
			{
				delta = border + shift - gridLine;
				position = border;
				gridLinePosition = gridLine;
			}
		}
		private double Pull(double shift, double factor, double delta, double position, double gridLinePosition, ref double accamulate, ref bool secondPass)
		{
			var result = shift;
			if (Math.Abs(delta) < factor)
			{
				if (position == gridLinePosition)
				{
					accamulate += shift;
					if (Math.Abs(position + accamulate - gridLinePosition) > factor)
					{
						result = accamulate;
						accamulate = 0;
						secondPass = true;
					}
					else
						result = 0;
				}
				else
				{
					accamulate = (position + shift) - gridLinePosition;
					result = gridLinePosition - position;
				}
			}
			else
				accamulate = 0;
			return result;
		}
	}
}
