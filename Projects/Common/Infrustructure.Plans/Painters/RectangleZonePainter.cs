using System.Windows;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;
using System.Windows.Media;

namespace Infrustructure.Plans.Painters
{
	public class RectangleZonePainter : ShapePainter<Rectangle>
	{
		public override UIElement Draw(ElementBase element)
		{
			var shape = CreateShape(element);
			shape.Opacity = 0.5;
			return shape;
		}
	}
}
