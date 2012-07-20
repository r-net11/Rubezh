using System.Windows;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class RectangleZonePainter : ShapePainter<Rectangle>
	{
		public override FrameworkElement Draw(ElementBase element)
		{
			var shape = CreateShape(element);
			shape.Opacity = 0.5;
			return shape;
		}
	}
}
