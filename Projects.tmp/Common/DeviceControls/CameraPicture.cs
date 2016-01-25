using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DeviceControls
{
	public class CameraPicture
	{
		private Geometry _geometry;
		private Dictionary<Brush, Brush> _brushes;

		internal CameraPicture()
		{
			_geometry = new CombinedGeometry()
			{
				Geometry1 = new RectangleGeometry(new Rect(0, 3, 14, 8), 1, 1),
				Geometry2 = new PathGeometry()
				{
					Figures = new PathFigureCollection()
					{
						new PathFigure()
						{
							StartPoint = new Point(16,5),
							IsClosed = false,
							Segments = new PathSegmentCollection()
							{
								new LineSegment(new Point(20,2),true),
								new LineSegment(new Point(20,12),true),
								new LineSegment(new Point(16,9),true),
							}
						}
					}
				}
			};
			_geometry.Freeze();
			_brushes = new Dictionary<Brush, Brush>();
		}

		public Brush GetDefaultBrush(bool withCamera = true)
		{
			return GetBrush(withCamera ? Brushes.Transparent : Brushes.Gray);
		}
		public Brush GetBrush(Brush backgroundBrush)
		{
			if (!_brushes.ContainsKey(backgroundBrush))
				AddBrush(backgroundBrush);
			return _brushes[backgroundBrush];
		}

		private void AddBrush(Brush brush)
		{
			var drawingBrush = new DrawingBrush()
			{
				Drawing = new GeometryDrawing()
				{
					Brush = brush,
					Pen = new Pen(Brushes.Black, 1),
					Geometry = _geometry
				}
			};
			drawingBrush.Freeze();
			_brushes.Add(brush, drawingBrush);
		}
	}
}