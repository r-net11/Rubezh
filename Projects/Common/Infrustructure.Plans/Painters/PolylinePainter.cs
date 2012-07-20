using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class PolylinePainter : ShapePainter<Polyline>
	{
		public override FrameworkElement Draw(ElementBase element)
		{
			var shape = CreateShape(element);
			shape.Fill = Brushes.Transparent;
			shape.Points = PainterHelper.GetPoints(element);
			return shape;
		}
	}
}
