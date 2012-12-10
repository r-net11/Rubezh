using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class PolylinePainter : ShapePainter<Polyline>
	{
		public override Visual Draw(ElementBase element)
		{
			var shape = CreateShape(element);
			shape.Fill = Brushes.Transparent;
			shape.Points = PainterHelper.GetPoints(element);
			return shape;
		}
	}
}
