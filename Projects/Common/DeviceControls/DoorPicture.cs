using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DeviceControls
{
	public class DoorPicture
	{
		private Pen _pen;
		private Geometry _geometry;
		private GeometryDrawing _geometryDrawing;
		private Dictionary<Brush, Brush> _brushes;

		internal DoorPicture()
		{
			_pen = new Pen(Brushes.Black, 2);
			_pen.Freeze();
			_geometry = new PathGeometry()
			{
				Figures = new PathFigureCollection()
				{
					new PathFigure()
					{
						IsClosed = true,
						IsFilled = true,
						Segments = new PathSegmentCollection()
						{
							new LineSegment(new Point(-40,0),true),
							new LineSegment(new Point(-40,100),true),
							new LineSegment(new Point(0,100),true),
						}
					}
				}
			};
			_geometry.Freeze();
			_geometryDrawing = new GeometryDrawing()
			{
				Brush = new SolidColorBrush(Colors.DarkOrange),
				Pen = _pen,
				Geometry = new PathGeometry()
				{
					Figures = new PathFigureCollection()
					{
						new PathFigure()
						{
							IsClosed = true,
							IsFilled = true,
							Segments = new PathSegmentCollection()
							{
								new LineSegment(new Point(20,40),true),
								new LineSegment(new Point(20,140),true),
								new LineSegment(new Point(0,100),true),
							}
						}
					}
				}
			};
			_geometryDrawing.Freeze();
			_brushes = new Dictionary<Brush, Brush>();
		}

		public Brush GetDefaultBrush(bool withDoor = true)
		{
			return GetBrush(withDoor ? Brushes.Transparent : Brushes.Gray);
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
				Drawing = new DrawingGroup()
				{
					Children = new DrawingCollection()
					{
						new GeometryDrawing()
						{
							Brush = brush,
							Pen = _pen,
							Geometry = _geometry
						},
						_geometryDrawing,
					}
				}
			};
			drawingBrush.Freeze();
			_brushes.Add(brush, drawingBrush);
		}
	}
}