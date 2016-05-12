using Infrastructure.Plans.Designer;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Infrastructure.Plans
{
	public class GridLineController : IGridLineController
	{
		private const double DELTA = 20;
		private double _accumulateX = 0;
		private double _accumulateY = 0;
		private BaseDesignerCanvas _canvas;
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

		public GridLineController(BaseDesignerCanvas canvas)
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
			_accumulateX = 0;
			_accumulateY = 0;
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
			System.Diagnostics.Debug.WriteLine(this._accumulateX + ", " + this._accumulateY);
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
				shift.X = Pull(shift.X, factor, deltaX, positionX, gridLinePositionX, ref _accumulateX);
				shift.Y = Pull(shift.Y, factor, deltaY, positionY, gridLinePositionY, ref _accumulateY);
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
		private double Pull(double shift, double factor, double delta, double position, double gridLinePosition, ref double accumulate)
		{
			var result = shift;
			if (Math.Abs(delta) < factor)
			{
				// Accumulating Shift Value:
				accumulate += shift;
				// Checking if accumulated Value is more than Grid Line's Factor:
				if (Math.Abs(position + accumulate - gridLinePosition) > factor)
				{
					// Moving the Element away from Grid Line:
					result = accumulate;
					accumulate = 0;
				}
				else
				{
					// Element is snapped to the Grid Line:
					result = gridLinePosition - position;
				}
			}
			else
				accumulate = 0;
			return result;
		}
	}
}
