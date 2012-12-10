using System.Windows;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class PolygonPainter : ShapePainter<Polygon>
	{
		public override Visual Draw(ElementBase element)
		{
			var shape = CreateShape(element);
			shape.Points = PainterHelper.GetPoints(element);
			return shape;
		}
	}
}
