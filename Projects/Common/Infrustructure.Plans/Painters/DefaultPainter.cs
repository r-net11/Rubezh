using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Painters
{
	public class DefaultPainter : ShapePainter<Rectangle>
	{
		#region IPainter Members

		public override Visual Draw(ElementBase element)
		{
			var shape = CreateShape(element);
			shape.Fill = Brushes.Black;
			return shape;
		}

		#endregion
	}
}