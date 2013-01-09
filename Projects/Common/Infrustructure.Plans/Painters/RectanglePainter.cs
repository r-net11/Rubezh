using System.Windows.Shapes;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using System.Windows;

namespace Infrustructure.Plans.Painters
{
	public class RectanglePainter : ShapePainter
	{
		protected override Geometry CreateShape(ElementBase element)
		{
			return new RectangleGeometry(Rect);
		}
	}
}
