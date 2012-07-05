using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class PolygonZonePainter : ShapePainter<Polygon>
	{
		public override FrameworkElement Draw(ElementBase element)
		{
			var shape = CreateShape(element);
			shape.Points = PainterHelper.GetPoints(element);
			shape.Opacity = 0.5;
			return shape;
		}
	}
}
