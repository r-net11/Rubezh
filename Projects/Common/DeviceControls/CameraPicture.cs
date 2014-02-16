using System;
using System.Collections.Generic;
using System.Windows.Media;
using FiresecClient;
using Infrustructure.Plans.Devices;
using XFiresecAPI;
using System.Windows;

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
				Geometry1 = new RectangleGeometry(new Rect(0, 0, 15, 10), 1, 1),
				Geometry2 = new PathGeometry()
				{
					Figures = new PathFigureCollection()
					{
						new PathFigure()
						{
							StartPoint = new Point(15,3),
							IsClosed = false,
							Segments = new PathSegmentCollection()
							{
								new LineSegment(new Point(20,1),true),
								new LineSegment(new Point(20,9),true),
								new LineSegment(new Point(15,7),true),
							}
						}
					}
				}
			};
			_geometry.Freeze();
			_brushes = new Dictionary<Brush, Brush>();
		}

		public Brush GetDefaultBrush()
		{
			return GetBrush(Brushes.Transparent);
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