using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Infrastructure.Client.Images
{
	public class WMFImage
	{
		public WMFImage()
		{
			Resources = new Dictionary<Guid, byte[]>();
		}

		public Canvas Canvas { get; set; }
		public Dictionary<Guid, byte[]> Resources { get; set; }

		public DrawingGroup ToDrawing()
		{
			var drawingGroup = new DrawingGroup();
			foreach (var child in Canvas.Children)
			{
				if (child is Path)
				{
					var path = (Path)child;
					var geometry = path.Data;
					var geometryDrawing = new GeometryDrawing
					{
						Geometry = path.Data.Clone(),
						Brush = path.Fill,
						Pen = new Pen(path.Stroke, path.StrokeThickness)
						{
							DashCap = path.StrokeDashCap,
							DashStyle = new DashStyle(path.StrokeDashArray, path.StrokeDashOffset),
							EndLineCap = path.StrokeEndLineCap,
							LineJoin = path.StrokeLineJoin,
							MiterLimit = path.StrokeMiterLimit,
							StartLineCap = path.StrokeStartLineCap,
						},
					};
					geometryDrawing.Geometry.Transform = path.RenderTransform;
					geometryDrawing.Geometry.Freeze();
					geometryDrawing.Freeze();
					drawingGroup.Children.Add(geometryDrawing);
				}
				else
					return null;
			}
			drawingGroup.Freeze();
			return drawingGroup;
		}
	}
}
